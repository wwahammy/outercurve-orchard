using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Orchard;

namespace Outercurve.Projects.Services
{
    public interface IImportFromOldExcelService : IDependency {
        
    }

    public class ImportFromOldExcelService : IImportFromOldExcelService
    {
        public void Import(SpreadsheetDocument doc) {
            var ourWorksheet = doc.WorkbookPart.WorksheetParts.First().Worksheet;
            var sheetData = ourWorksheet.Elements<SheetData>().First();
            foreach (var r in sheetData.Elements<Row>()) {
                foreach (var c in r.Elements<Cell>().OrderBy(c => c.CellReference).Select((c, i) => new {Cell = c, Index = i})) {
                  
                   if (c.Cell.DataType == CellValues.SharedString && String.IsNullOrWhiteSpace(c.Cell.CellValue.Text)) {
                       
                      var text = doc.GetSharedStringTablePart().GetSharedStringFromIndex(int.Parse(c.Cell.CellValue.Text));
                     
                   }
                   
                }
            }

        }


    }
}