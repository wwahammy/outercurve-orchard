using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Outercurve.Projects.Models;
using Outercurve.Projects.MoreLinq;

namespace Outercurve.Projects.Services
{
    public interface ICLAToOfficeService : IDependency {
        byte[] CreateCLASpreadsheet();
    }

    public class CLAToOfficeService : ICLAToOfficeService
    {
        private readonly IContentManager _contentManager;

        public CLAToOfficeService(IContentManager contentManager) {
            _contentManager = contentManager;
        }
       
        public byte[] CreateCLASpreadsheet() {

            using (var memStream = new MemoryStream()) {

                SpreadsheetDocument doc = SpreadsheetDocument.Create(memStream, SpreadsheetDocumentType.Workbook);
                var workbookPart = doc.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                newWorksheetPart.Worksheet = CreateAndFillWorksheet(doc);
                newWorksheetPart.Worksheet.Save();

                Sheets sheets = doc.WorkbookPart.Workbook.AppendChild(new Sheets());
                string relationshipId = doc.WorkbookPart.GetIdOfPart(newWorksheetPart);


                // Give the new worksheet a name.
                string sheetName = "Sheet1";

                // Append the new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet() { Id = relationshipId, SheetId = 1, Name = sheetName };
                sheets.Append(sheet);
                doc.WorkbookPart.Workbook.Save();
                
                doc.Close();

                return memStream.ToArray();
            }
        }

        
        private Worksheet CreateAndFillWorksheet(SpreadsheetDocument doc) {
            
            var sheetData = new SheetData();


            CreateAndAddHeaderRow(sheetData, doc);

            AddRows(sheetData, doc);
            var ret = new Worksheet(sheetData);
            return ret;
        }

        private Row CreateAndAddHeaderRow(SheetData sheetData, SpreadsheetDocument doc ) {
            var row = new Row {RowIndex = 1};
            sheetData.InsertAt(row, 0);

            var headerNames = new[] {
                "Project",
                "CLA Signer",
                "Is Signed",
                "Signed Date",
                "Has Foundation Signer",
                "Foundation Signer",
                "Foundation Signer Date",
                "Has Employer Signer",
                "Signer From Employer",
                "Employer Signed Date",
                "Employer",
                "Location of CLA",
                "Is Valid",
                "Is Committer",
                "Comments"
            };

            foreach (var header in headerNames) {
                var newCell = new Cell();
                newCell.SetText(header, doc);
                row.AddCellEndOfRow(newCell);
            }

            return row;
        }


        private void AddRows(SheetData sheetData, SpreadsheetDocument doc) {
            var previousRow = sheetData.GetFirstChild<Row>();
            var allTheContent = _contentManager.Query().ForType("CLA").List();
           
           

             allTheContent.Aggregate(previousRow, (current, content) => AddRow(content, sheetData, current, doc));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="previousRow"></param>
        /// <returns>the newly created row</returns>
        private Row AddRow(IContent item, SheetData sheetData, Row previousRow, SpreadsheetDocument doc) {
           var newRow = new Row {RowIndex = previousRow.RowIndex + 1};

          

            var claPart = item.As<CLAPart>();
            var rowContents = new [] {
                item.As<CommonPart>().Container.As<TitlePart>().Title,
                claPart.CLASigner.FullName(),
                claPart.IsSignedByUser.ToString(),
                claPart.IsSignedByUser && claPart.SignedDate != null ? claPart.SignedDate.Value.ToLocalTime().ToString("d") : "",
                claPart.HasFoundationSigner.ToString(),
                claPart.HasFoundationSigner && claPart.FoundationSigner != null ? claPart.FoundationSigner.FullName() : "",
                claPart.HasFoundationSigner && claPart.FoundationSignedOn != null ? claPart.FoundationSignedOn.Value.ToLocalTime().ToString("d") : "",
                claPart.RequiresEmployerSigner.ToString(),
                claPart.SignerFromCompany,
                claPart.RequiresEmployerSigner && claPart.EmployerSignedOn != null ? claPart.EmployerSignedOn.Value.ToLocalTime().ToString("d") : "",
                claPart.Employer,
                claPart.LocationOfCLA,
                claPart.IsValid.ToString(),
                claPart.IsCommitter.ToString(),
                claPart.Comments
            };

            foreach (var cell in rowContents.Select(c => c ?? "")) {
                var newCell = new Cell();
                var value = cell;
                newCell.SetText(value, doc);
                newRow.AddCellEndOfRow(newCell);
            }
            sheetData.InsertAfter(newRow, previousRow);
            return newRow;

        }

       
        
    }


    public static class SpreadsheetExtensions {

        private static readonly Regex ColumnAndRowFromCellReference = new Regex("^(?<column>[A-Z]+)(?<row>[0-9]+)$");
        
        public static Cell AddCellEndOfRow(this Row row, Cell cell) {
            var cells = row.OfType<Cell>().ToArray();
            var hasCells = row.OfType<Cell>().Any();

            if (!hasCells) {
                cell.CellReference = "A" + row.RowIndex;
               return row.InsertAt(cell, 0);
                
            }
            else {
                var lastCell = cells.MaxBy(c => new int? (GetColumnNumber(GetColumnAndRowFromCellReference(c.CellReference.Value).Column)));
                var lastCellColumnRow = GetColumnAndRowFromCellReference(lastCell.CellReference);
                cell.CellReference = GetExcelColumnName(GetColumnNumber(lastCellColumnRow.Column) + 1) + row.RowIndex;
                return row.InsertAfter(cell, lastCell);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        /// <from>http://stackoverflow.com/questions/181596/how-to-convert-a-column-number-eg-127-into-an-excel-column-eg-aa</from>
        public static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <from>http://stackoverflow.com/questions/848147/how-to-convert-excel-sheet-column-names-into-numbers</from>
        public static int GetColumnNumber(string name)
        {
            int number = 0;
            int pow = 1;
            for (int i = name.Length - 1; i >= 0; i--)
            {
                number += (name[i] - 'A' + 1) * pow;
                pow *= 26;
            }

            return number;
        }

        public static ColumnAndRow GetColumnAndRowFromCellReference(string cellReference) {
            var cellRefMatch = ColumnAndRowFromCellReference.Match(cellReference);
            return new ColumnAndRow {Column = cellRefMatch.Groups["column"].Value, Row = cellRefMatch.Groups["Row"].Value};
        }


        public static void SetText(this Cell cell, string text, SpreadsheetDocument doc) {
            cell.CellValue = new CellValue(doc.GetSharedStringTablePart().InsertSharedStringItem(text).ToString(CultureInfo.InvariantCulture));
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        }

        public static SharedStringTablePart GetSharedStringTablePart(this SpreadsheetDocument doc) {
            return doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Any() ? doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First() : doc.WorkbookPart.AddNewPart<SharedStringTablePart>();
        }

        public static int InsertSharedStringItem(this SharedStringTablePart shareStringPart, string text) {
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }


        public static string GetSharedStringFromIndex(this SharedStringTablePart sharedString, int index) {
           
            foreach (var item in sharedString.SharedStringTable.Elements<SharedStringItem>().Select((s, i) => new {Item = s, Index = i}))
            {
                if (item.Index == index) {
                    return item.Item.InnerText;
                }
                
            }

            return null;
        }

        public static int GetSharedStringIndex(this SharedStringTablePart sharedString, string text) {
         
            foreach (var item in sharedString.SharedStringTable.Elements<SharedStringItem>().Select((s, i) => new {Item = s, Index = i})) {
                if (item.Item.InnerText == text)
                {
                    return item.Index;
                }
                
            }

            return -1;
        }

    }
    public class ColumnAndRow {
        public string Column { get; set; }
        public string Row { get; set; }
    }

}