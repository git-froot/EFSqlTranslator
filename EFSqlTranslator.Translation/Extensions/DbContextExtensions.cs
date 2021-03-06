﻿using System.Collections.Generic;
using System.Linq;
using Dapper;
using EFSqlTranslator.Translation.DbObjects;
using Microsoft.EntityFrameworkCore;

namespace EFSqlTranslator.Translation.Extensions
{
    public static class DbContextExtensions
    {
        public static IEnumerable<T> Query<T>(this DbContext db,
            IQueryable<T> query, IModelInfoProvider infoProvider, IDbObjectFactory factory)
        {
            var executor = LinqExecutorMaker.Make(query, infoProvider, factory, db);
            var result = executor.Execute();

            return result;
        }

        public static IEnumerable<dynamic> QueryDynamic(this DbContext db,
            IQueryable query, IModelInfoProvider infoProvider, IDbObjectFactory factory)
        {
            using (var connection = db.Database.GetDbConnection())
            {
                var script = QueryTranslator.Translate(query.Expression, infoProvider, factory);
                var sql = script.ToString();

                var results = connection.Query(sql);
                return results;
            }
        }

        public static IEnumerable<T> Query<T>(this DbContext db,
            IQueryable<T> query, IModelInfoProvider infoProvider, IDbObjectFactory factory, out string sql)
        {
            var executor = LinqExecutorMaker.Make(query, infoProvider, factory, db);
            sql = executor.Script.ToString();
            
            var result = executor.Execute();
            return result;
        }

        public static IEnumerable<dynamic> QueryDynamic(this DbContext db,
            IQueryable query, IModelInfoProvider infoProvider, IDbObjectFactory factory, out string sql)
        {
            using (var connection = db.Database.GetDbConnection())
            {
                var script = QueryTranslator.Translate(query.Expression, infoProvider, factory);
                sql = script.ToString();

                var results = connection.Query(sql);
                return results;
            }
        }
    }
}