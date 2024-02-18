﻿using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Common;

namespace Hub.Services.Common
{
   /// <summary>
   /// Search term service interface
   /// </summary>
   public partial interface ISearchTermService
   {
      /// <summary>
      /// Gets a search term record by keyword
      /// </summary>
      /// <param name="keyword">Search term keyword</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the search term
      /// </returns>
      Task<SearchTerm> GetSearchTermByKeywordAsync(string keyword);

      /// <summary>
      /// Gets a search term statistics
      /// </summary>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains a list search term report lines
      /// </returns>
      Task<IPagedList<SearchTermReportLine>> GetStatsAsync(int pageIndex = 0, int pageSize = int.MaxValue);

      /// <summary>
      /// Inserts a search term record
      /// </summary>
      /// <param name="searchTerm">Search term</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertSearchTermAsync(SearchTerm searchTerm);

      /// <summary>
      /// Updates the search term record
      /// </summary>
      /// <param name="searchTerm">Search term</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task UpdateSearchTermAsync(SearchTerm searchTerm);
   }
}