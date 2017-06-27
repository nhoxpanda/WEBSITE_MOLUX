using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.DataValidation.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using TOURDEMO.Models;

namespace TOURDEMO.Utilities
{
    public static class ExcelUtility
    {
        //supposed to header code row index (hidden) is at row 10
        public static int COLUMN_CODE_ROW_INDEX = 6;

        public static void AddHeader(this ExcelWorksheet ws, IDictionary<string, string> header)
        {
            if (header == null || header.Keys.Count == 0)
                return;
            try
            {
                ws.Cells["C4"].Value = header["TOURCODE"];
            }
            catch { }
            try
            {
                ws.Cells["C5"].Value = header["FLIGHTDETAIL"];
            }
            catch { }
            try
            {
                ws.Cells["A6"].Value = header["TOURNAME"];
            }
            catch { }
        }

        public static IDictionary<int, string> GetColumnTitles(this ExcelWorksheet worksheet, int headerRowIndex)
        {
            IDictionary<int, string> columnTitles = new Dictionary<int, string>();
            int iTitleCol = 1;
            while (true)
            {
                var title = worksheet.Cells[headerRowIndex, iTitleCol].Value as string;
                if (string.IsNullOrEmpty(title))
                    break;
                columnTitles.Add(iTitleCol, title);
                //increase column index
                iTitleCol++;
            }
            return columnTitles;
        }

        public static int GetColumnIndex(this ExcelWorksheet ws, string columnName)
        {
            int columnIndex = 1;
            for (;  columnIndex < 1000; columnIndex++)
            {
                var headerCode = ws.Cells[COLUMN_CODE_ROW_INDEX, columnIndex].Value as string;
                if (string.IsNullOrEmpty(headerCode))
                    continue;
                if (headerCode.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    break;
            }
            return columnIndex % 1000;
        }

        public static void AddColumnCode(this ExcelWorksheet ws, int columnIndex, string headerCodeName)
        {
            ws.Cells[COLUMN_CODE_ROW_INDEX, columnIndex].Value = headerCodeName;
        }

        public static void AddColumnTitle(this ExcelWorksheet ws, int columnIndex, string title)
        {
            ws.Cells[COLUMN_CODE_ROW_INDEX - 1, columnIndex].Value = title;
        }

        public static void AddComment(this ExcelWorksheet ws, int rowIndex, int colIndex, string comment, string author)
        {
            ws.Cells[rowIndex, colIndex].AddComment(comment, author);
        }

        public static void AddFormula(this ExcelWorksheet ws, string address, string formula)
        {
            ws.Cells[address].Formula = formula;
        }

        public static void AddFormula(this ExcelWorksheet ws, int rowIndex, int colIndex, string formula)
        {
            ws.Cells[rowIndex, colIndex].Formula = formula;
        }

        public static void Hide(this ExcelWorksheet ws, int colIndex)
        {
            ws.Column(colIndex).OutlineLevel = 1;
            ws.Column(colIndex).Collapsed = true;
            ws.OutLineSummaryRight = true;
        }

        public static IExcelDataValidationDateTime AddDateTimeValidation(this ExcelWorksheet ws, int columnIndex, string errorTitle, string errorMessage,
            ExcelDataValidationOperator @operator = ExcelDataValidationOperator.greaterThanOrEqual, DateTime? firstFormulaValue = null, DateTime? secondFormulaValue = null)
        {
            int rowIndex = COLUMN_CODE_ROW_INDEX + 1;
            var rangeAddress = ws.Cells[rowIndex, columnIndex, rowIndex + 1000, columnIndex].Address;
            var val = ws.DataValidations.AddDateTimeValidation(rangeAddress);

            val.ShowErrorMessage = true;
            val.Error = errorMessage;
            val.ErrorTitle = errorTitle;
            val.PromptTitle = "Nhập ngày tháng";
            val.Prompt = "Nhập ngày hợp lệ lớn hơn 1/1/1900.";
            val.Operator = @operator;
            switch (@operator)
            {
                case ExcelDataValidationOperator.equal:
                case ExcelDataValidationOperator.notEqual:
                case ExcelDataValidationOperator.greaterThan:
                case ExcelDataValidationOperator.greaterThanOrEqual:
                case ExcelDataValidationOperator.lessThan:
                case ExcelDataValidationOperator.lessThanOrEqual:
                    if (firstFormulaValue == null)
                        firstFormulaValue = DateTime.MinValue;
                    val.Formula.Value = firstFormulaValue;
                    break;
                case ExcelDataValidationOperator.between:
                case ExcelDataValidationOperator.notBetween:
                    val.Formula.Value = firstFormulaValue;
                    val.Formula2.Value = secondFormulaValue;
                    break;
                default:
                    break;
            }
            return val;
        }

        private static IExcelDataValidationList AddListValidation(this ExcelWorksheet ws, string address, string errorTitle, string errorMessage, IList<string> values)
        {
            var val = ws.DataValidations.AddListValidation(address);
            val.ShowErrorMessage = true;
            val.Error = errorMessage;
            val.ErrorTitle = errorTitle;
            foreach (var value in values)
            {
                val.Formula.Values.Add(value);
            }
            return val;
        }

        private static IExcelDataValidationList AddListValidation(this ExcelWorksheet ws, string address, string errorTitle, string errorMessage, string namedRange)
        {
            var val = ws.DataValidations.AddListValidation(address);
            val.ShowErrorMessage = true;
            val.Error = errorMessage;
            val.ErrorTitle = errorTitle;
            val.Formula.ExcelFormula = string.Format("={0}", namedRange);
            return val;
        }

        public static void AddListValidation(this ExcelWorksheet ws, ExcelWorksheet valWs, IEnumerable<ExportItem> validationItems, int columnIndex, string errorTitle, string errorMessage, string rangeName, string textRangeName)
        {
            if (validationItems == null || validationItems.Count() == 0)
                //throw new ArgumentException("validation data is null");
                return;

            if (columnIndex == 0)
                return;

            int rowIndex = COLUMN_CODE_ROW_INDEX;
            if (!ws.Names.ContainsKey(rangeName))
            {
                var range = valWs.Cells[rowIndex, columnIndex].LoadFromCollection(validationItems);
                ws.Names.Add(rangeName, range);
                ws.Names.Add(textRangeName, range.Offset(0, 0, validationItems.Count(), 1));
            }
            var rangeAddress = ws.Cells[rowIndex, columnIndex, rowIndex + 1000, columnIndex].Address;
            ws.AddListValidation(rangeAddress, errorTitle, errorMessage, textRangeName);
            //ws.Cells[rowIndex, columnIndex, rowIndex + 1000, columnIndex].FormulaR1C1
            //    = string.Format("=VLOOKUP({0}, {1}, {2}, FALSE)", "RC[+1]", rangeName, 2);
        }

        //public static IExcelDataValidationList AddListValidation(this ExcelWorksheet ws, string address, string errorTitle, string errorMessage, string referenceAddress)
        //{
        //    var val = ws.DataValidations.AddListValidation(address);
        //    val.ShowErrorMessage = true;
        //    val.Error = errorMessage;
        //    val.ErrorTitle = errorTitle;
        //    val.Formula.ExcelFormula = string.Format("=INDIRECT{0}", referenceAddress);
        //    return val;
        //}

        //protected static void AddValidationData(this ExcelWorksheet ws, IEnumerable<object> data, int columnIndex, string rangeName, string textRangeName)
        //{
        //    var range = ws.Cells[1, columnIndex].LoadFromCollection(data);
        //    ws.Names.Add(rangeName, range);
        //    ws.Names.Add(textRangeName, range.Offset(0, 1, data.Count(), 1));
        //}
    }
}