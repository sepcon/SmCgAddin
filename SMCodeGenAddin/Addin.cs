using System;
using System.Windows.Forms;

namespace SMCodeGenAddin
{
    public class Addin
    {
        // define menu constants
        const string menuHeader = "-&StateMachine Code Generation";
        const string naiveGenerator = "&Generate Source with function naming by numbering";
        const string simpleTransAndConds = "&Generate Source for diagrams with simple transition conditions and actions";


        ///
        /// Called Before EA starts to check Add-In Exists
        /// Nothing is done here.
        /// This operation needs to exists for the addin to work
        ///
        /// <param name="Repository" />the EA repository
        /// a string
        public String EA_Connect(EA.Repository Repository)
        {
            //No special processing required.
            return "a string";
        }

        ///
        /// Called when user Clicks Add-Ins Menu item from within EA.
        /// Populates the Menu with our desired selections.
        /// Location can be "TreeView" "MainMenu" or "Diagram".
        ///
        /// <param name="Repository" />the repository
        /// <param name="Location" />the location of the menu
        /// <param name="MenuName" />the name of the menu
        ///
        public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName)
        {

            switch (MenuName)
            {
                // defines the top level menu option
                case "":
                    return menuHeader;
                // defines the submenu options
                case menuHeader:
                    string[] subMenus = { naiveGenerator, simpleTransAndConds };
                    return subMenus;
            }
            return "";
        }

        ///
        /// returns true if a project is currently opened
        ///
        /// <param name="Repository" />the repository
        /// true if a project is opened in EA
        bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool shouldEnableMenu(EA.Repository Repository, string Location)
        {
            return IsProjectOpen(Repository) && isStateChart(getCurrentDiagram(Repository, Location));
        }

        private EA.Diagram getCurrentDiagram(EA.Repository Repository, string Location)
        {
            EA.Diagram currentDg = null;
            switch (Location)
            {
                case  "MainMenu":
                case "Diagram":
                {
                    currentDg = Repository.GetCurrentDiagram();
                }
                    break;
                case "TreeView":
                {
                    object selectedItem;
                    var itemType = Repository.GetTreeSelectedItem(out selectedItem);
                    if(itemType == EA.ObjectType.otDiagram)
                    {
                        currentDg = (EA.Diagram)selectedItem;
                    }
                }
                break;

                default: break;
            }
            return currentDg;
        }
        private bool isStateChart(EA.Diagram dg)
        {
            return dg != null && dg.Type.ToString().ToLower() == "statechart";
        }

        ///
        /// Called once Menu has been opened to see what menu items should active.
        ///
        /// <param name="Repository" />the repository
        /// <param name="Location" />the location of the menu
        /// <param name="MenuName" />the name of the menu
        /// <param name="ItemName" />the name of the menu item
        /// <param name="IsEnabled" />boolean indicating whethe the menu item is enabled
        /// <param name="IsChecked" />boolean indicating whether the menu is checked
        public void EA_GetMenuState(EA.Repository Repository, string Location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            IsEnabled = shouldEnableMenu(Repository, Location);
        }

        ///
        /// Called when user makes a selection in the menu.
        /// This is your main exit point to the rest of your Add-in
        ///
        /// <param name="Repository" />the repository
        /// <param name="Location" />the location of the menu
        /// <param name="MenuName" />the name of the menu
        /// <param name="ItemName" />the name of the selected menu item
        public void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            EA.Diagram currentDiagram = getCurrentDiagram(Repository, Location);
            switch (ItemName)
            {
                // user has clicked the menuHello menu option
                case naiveGenerator:
                    executeFunction(Repository, currentDiagram, ARSourceGeneration.FunctionNamerType.Numbering);
                    break;
                case simpleTransAndConds:
                    executeFunction(Repository, currentDiagram, ARSourceGeneration.FunctionNamerType.SimpleTransAndConds);
                    //MessageBox.Show("The function hasn't implemented yet");
                    break;
            }
        }

        private void executeFunction(EA.Repository Repository, EA.Diagram currentDiagram, ARSourceGeneration.FunctionNamerType fnType, bool oneEntryExitPerState = true)
        {
            var foderBrowser = new FolderBrowserDialog();
            foderBrowser.Description = "Select Output Directory for generating code";
            DialogResult result = foderBrowser.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(foderBrowser.SelectedPath))
            {
                ARSourceGeneration.Project sourceProject = new ARSourceGeneration.Project(Repository, currentDiagram, fnType, oneEntryExitPerState);
                //sourceProject.changeChartParser(new EAParsing.StateChartParserV1());
                try
                {
                    sourceProject.generateSource(foderBrowser.SelectedPath);
                    showResultMessage(sourceProject);
                }
                catch (System.Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        void showResultMessage(ARSourceGeneration.Project sourceOutput)
        {
            string message = ARSourceGeneration.FormatGetter.get(ARSourceGeneration.ErrorMessageFormatIndex.GeneratingCodeFailed);
            string[] outputFiles = sourceOutput.getOutPutFiles();
            if (outputFiles != null)
            {
                message = string.Format(
                    ARSourceGeneration.FormatGetter.get(ARSourceGeneration.ErrorMessageFormatIndex.GeneratingCodeSuccessful),
                    sourceOutput.ProjectName,
                    outputFiles[0],
                    outputFiles[1]);
            }

            if (ARSourceGeneration.MessageLogger.NumberOfErrorMessages > 0)
            {
                message += "\nThere are " + ARSourceGeneration.MessageLogger.NumberOfErrorMessages + " ERRORS detected!\n" + "For details see the log file: " + ARSourceGeneration.MessageLogger.LogFile;

            }

            MessageBox.Show(message);
        }

        ///
        /// EA calls this operation when it exists. Can be used to do some cleanup work.
        ///
        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
