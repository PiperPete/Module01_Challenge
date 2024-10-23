using Autodesk.Revit.UI.Selection;

namespace Module01_Challenge
{
    [Transaction(TransactionMode.Manual)]
    public class Module01_Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here
            // 1. set variables
            int numFloors = 250;
            double startElev = 0;
            int floorHeight = 16;
            int fizzbuzzCount = 0;
            int fizzCount = 0;
            int buzzCount = 0;

            // collects titleblocks
            FilteredElementCollector tbcollector = new FilteredElementCollector(doc);
            tbcollector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            tbcollector.WhereElementIsElementType();
            ElementId tblockId = tbcollector.FirstElementId();

            //collects floor plans
            FilteredElementCollector vftCollector = new FilteredElementCollector(doc);
            vftCollector.OfClass(typeof(ViewFamilyType));

            ViewFamilyType fpVFT = null;
            ViewFamilyType cpVFT = null;

            foreach (ViewFamilyType curVFT in vftCollector)
            {
                if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    fpVFT = curVFT;
                }
                else if (curVFT.ViewFamily == ViewFamily.CeilingPlan)
                {
                    cpVFT = curVFT;
                }
            }

            Transaction t = new Transaction(doc);
            t.Start("FIZZBUZZ Challenge");

            for (int i = 1; i <= numFloors; i++)
            {
                Level newLevel = Level.Create(doc, startElev);
                newLevel.Name = "LEVEL " + i.ToString();

                startElev += floorHeight;

                if (i % 3 == 0 && i % 5 == 0)
                {
                    ViewSheet newSheet = ViewSheet.Create(doc, tblockId);
                    newSheet.SheetNumber = i.ToString();
                    newSheet.Name = "FIZZBUZZ_#" + i.ToString();
                    ViewPlan bonusPlan = ViewPlan.Create(doc, fpVFT.Id, newLevel.Id);
                    bonusPlan.Name = "FIZZBUZZ_#" + i.ToString();

                    Viewport newVP = Viewport.Create(doc, newSheet.Id,bonusPlan.Id,new XYZ(1.25,1,0));
                    fizzbuzzCount++;
                }
                else if (i % 3 == 0)
                {
                    ViewPlan newPlan = ViewPlan.Create(doc, fpVFT.Id, newLevel.Id);
                    newPlan.Name = "FIZZ_#" + i.ToString();
                    fizzCount++;
                }
                else if (i % 5 == 0)
                {
                    ViewPlan newPlan = ViewPlan.Create(doc, cpVFT.Id, newLevel.Id);
                    newPlan.Name = "BUZZ_#" + i.ToString();
                    buzzCount++;
                }
            }
            t.Commit();
            t.Dispose();

            string resultString = $"Created{numFloors} Levels. {fizzbuzzCount} FIZZBUZZ. {fizzCount} FIZZ.{buzzCount} BUZZ.";
            TaskDialog.Show("Complete", resultString);

            return Result.Succeeded;

        }
    }
} 
