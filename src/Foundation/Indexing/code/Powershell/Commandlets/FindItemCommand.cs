using Cognifide.PowerShell.Core.Extensions;
using Cognifide.PowerShell.Core.VersionDecoupling;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Security;
using Sitecore.ContentSearch.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Cognifide.PowerShell.Commandlets.Data.Search;

namespace PSWebsite.Foundation.Indexing.Powershell.Commandlets
{
    [Cmdlet("Find", "CustomItem", DefaultParameterSetName = "Criteria")]
    [OutputType(typeof(SearchResultItem))]
    public class FindItemCommand : Cognifide.PowerShell.Commandlets.Data.Search.FindItemCommand
    {
        [Parameter]
        public Type Type { get; set; }

        protected override void EndProcessing()
        {
            // Do the base logic if no Type is passed
            // Type must inherit from SearchResultItem
            if (Type == null || Type == typeof(SearchResultItem) || !Type.IsSubclassOf(typeof(SearchResultItem)))
            {
                base.EndProcessing();
                return;
            }

            using (IProviderSearchContext searchContext = ContentSearchManager.GetIndex(string.IsNullOrEmpty(Index) ? "sitecore_master_index" : Index).CreateSearchContext(SearchSecurityOptions.Default))
            {
                var objType = (dynamic)Activator.CreateInstance(Type);
                var query = GetQueryable(objType, searchContext);

                if (!string.IsNullOrEmpty(Where))
                    query = WhereAndValues(objType, query);

                if (Criteria != null) {
                    // TODO: Implement generic - will break the search if it's using custom properties
                    var predicate = ProcessCriteria(Criteria, SearchOperation.And);
                    query = query.Where(predicate);
                }
                if (Predicate != null)
                {
                    // TODO: Implement generic - will break the search if it's using custom properties
                    query = query.Where(Predicate);
                }
                if (ScopeQuery != null)
                {
                    // TODO: Implement generic - will break the search if it's using custom properties
                    query = ProcessScopeQuery(searchContext, ScopeQuery);
                }
                if (!string.IsNullOrEmpty(OrderBy))
                    SitecoreVersion.V75.OrNewer(() => query = OrderIfSupported(objType, query))
                        .ElseWriteWarning(this, "OrderBy", true);

                WriteObject(FilterByPosition(query), true);
            }
        }

        /// <summary>
        /// Generic GetQueryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objType"></param>
        /// <param name="searchContext"></param>
        /// <returns></returns>
        private IQueryable<T> GetQueryable<T>(T objType, IProviderSearchContext searchContext)
        {
            return searchContext.GetQueryable<T>();
        }

        /// <summary>
        /// Generic WhereAndValues
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objType"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private IQueryable<T> WhereAndValues<T>(T objType, IQueryable<T> query)
        {
            SitecoreVersion.V75
                .OrNewer(() =>
                    query = query.Where(Where, WhereValues.BaseArray()))
                .ElseWriteWarning(this, "Where", true);
            return query;
        }

        /// <summary>
        /// Generic OrderIfSupported
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objType"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private IQueryable<T> OrderIfSupported<T>(T objType, IQueryable<T> query)
        {
            query = query.OrderBy(OrderBy);
            return query;
        }

        /// <summary>
        /// Generic FilterByPosition
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private List<SearchResultItem> FilterByPosition(IQueryable<SearchResultItem> query)
        {
            var num1 = query.Count();
            var flag = Last != 0 && First == 0;
            var count1 = flag ? 0 : Skip;
            var count2 = First;
            if (Last == 0 && First == 0)
                count2 = num1 - count1;
            var source = query.Skip(count1).Take(count2);
            var num2 = count1 + count2;
            if (num2 >= num1 || Last == 0 || flag && Skip >= num1 - num2)
                return source.ToList();
            var num3 = Last + (flag ? Skip : 0);
            var num4 = num1 - num2 - num3;
            var last = Last;
            if (num4 >= 0)
                return source.ToList().Concat(query.Skip(num2 + num4).Take(last)).ToList();
            var count3 = last + num4;
            var num5 = 0;
            return source.ToList().Concat(query.Skip(num2 + num5).Take(count3)).ToList();
        }
    }
}