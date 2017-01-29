﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EFSqlTranslator.Translation.DbObjects;

namespace EFSqlTranslator.Translation
{
    public class TranslationState
    {
        public Stack<IDbObject> ResultStack { get; } = new Stack<IDbObject>();

        public Stack<Dictionary<ParameterExpression, DbReference>> ParamterStack { get; } =
            new Stack<Dictionary<ParameterExpression, DbReference>>();

        public Dictionary<Tuple<IDbSelect, EntityRelation>, IDbJoin> CreatedJoins { get; } =
            new Dictionary<Tuple<IDbSelect, EntityRelation>, IDbJoin>();

        public List<string> IncludeSplits { get; } = new List<string>();

        public IDbSelect GetLastSelect()
        {
            IDbSelect dbSelect = null;

            var results = new Stack<IDbObject>();
            while(ResultStack.Count > 0)
            {
                var dbObject = ResultStack.Pop();
                results.Push(dbObject);

                dbSelect = dbObject as IDbSelect;
                if (dbSelect != null)
                    break;
            }

            while(results.Count > 0)
                ResultStack.Push(results.Pop());

            return dbSelect;
        }
    }
}