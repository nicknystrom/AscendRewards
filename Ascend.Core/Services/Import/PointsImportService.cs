using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Core.Services.Import
{
    public enum PointsColumnMappingTargets
    {
        None = -1,
        EmployeeId = 0,
        Login,
        Amount,
    }

    public class PointsImportService : BaseImportService<Transaction, PointsColumnMappingTargets>
    {

        public override ImportResult ValidateLayout(PointsColumnMappingTargets[] layout)
        {
            var result = new ImportResult();
            if (!layout.Any(x => x == PointsColumnMappingTargets.Amount))
            {
                result.AddProblem("Amount is a required field.");
            }
            if (!layout.Any(x => x == PointsColumnMappingTargets.EmployeeId) &&
                !layout.Any(x => x == PointsColumnMappingTargets.Login))
            {
                result.AddProblem("Either EmployeeId or Login fields are required.");
            }
            return result;
        }

        public override void ValidateRow(ImportRow row)
        {

        }

        public override Transaction Find(ImportRow row)
        {
            return null;
        }

        public override Transaction Create(ImportRow row)
        {
            return new Transaction
            {
            };
        }

        public override void Save(Transaction entity)
        {
        }

        public override ImportDataType GetColumnType(PointsColumnMappingTargets x)
        {
            switch (x)
            {
                case PointsColumnMappingTargets.Amount:
                    return ImportDataType.Number;

                default:
                    return ImportDataType.String;
            }    
        }

        public override void Apply(Transaction p, ImportRow r)
        {
            for (var i=0; i<r.Layout.Length; i++)
            {
                var value = r.Values[i];
                var target = r.Layout[i];
    
                switch (target)
                {
                    case PointsColumnMappingTargets.Amount: p.Amount = (int)value; break;

                }
            }
        }

        void ApplyCustom(string custom, string value, Transaction p)
        {
        }
    }
}
