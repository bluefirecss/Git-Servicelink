

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServicelinkASAPMobile.Utilities;
using SQLite;

namespace ServicelinkASAPMobile.Data
{
	/// <summary>
	/// A helper class for working with SQLite
	/// </summary>
	public static class Database
	{
		#if NETFX_CORE
		private static readonly string Path = "Database.db"; //TODO: change this later
		#elif NCRUNCH
		private static readonly string Path = System.IO.Path.GetTempFileName();
		#else
		private static readonly string Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Database.db");
		#endif
		private static bool initialized = false;
		private static readonly Type [] tableTypes = new Type []
		{
			typeof(Posting),
			typeof(Photo),
			typeof(Document),
			typeof(User),
			typeof(Signature),

		};

		/// <summary>
		/// For use within the app on startup, this will create the database
		/// </summary>
		/// <returns></returns>
		public static Task Initialize (CancellationToken cancellationToken)
		{
			return CreateDatabase(new SQLiteAsyncConnection(Path, true), cancellationToken);
		}

		/// <summary>
		/// Global way to grab a connection to the database, make sure to wrap in a using
		/// </summary>
		public static SQLiteAsyncConnection GetConnection (CancellationToken cancellationToken)
		{
			var connection = new SQLiteAsyncConnection(Path, true);
			if (!initialized)
			{
				CreateDatabase(connection, cancellationToken).Wait();
			}
			return connection;
		}

		private static Task CreateDatabase (SQLiteAsyncConnection connection, CancellationToken cancellationToken)
		{
			return Task.Factory.StartNew(() =>
				{
					//Create the tables
					var createTask = connection.CreateTablesAsync (tableTypes);
					createTask.Wait();
					/*
					//Count number of assignments
					var countTask = connection.Table<Assignment>().CountAsync();
					countTask.Wait();

					//If no assignments exist, insert our initial data
					if (countTask.Result == 0)
					{
						var insertTask = connection.InsertAllAsync(TestData.All);

						//Wait for inserts
						insertTask.Wait();

						//Mark database created
						initialized = true;
					}
					*/
				});
		}
	}
}
