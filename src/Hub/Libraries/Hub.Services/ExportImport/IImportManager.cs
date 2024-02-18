﻿using System.IO;
using System.Threading.Tasks;

namespace Hub.Services.ExportImport
{
   /// <summary>
   /// Import manager interface
   /// </summary>
   public partial interface IImportManager
   {
      /// <summary>
      /// Import newsletter subscribers from TXT file
      /// </summary>
      /// <param name="stream">Stream</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the number of imported subscribers
      /// </returns>
      Task<int> ImportNewsletterSubscribersFromTxtAsync(Stream stream);

      /// <summary>
      /// Import states from TXT file
      /// </summary>
      /// <param name="stream">Stream</param>
      /// <param name="writeLog">Indicates whether to add logging</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the number of imported states
      /// </returns>
      Task<int> ImportStatesFromTxtAsync(Stream stream, bool writeLog = true);
   }
}
