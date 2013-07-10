using System;
using System.IO;
using System.Text;
using System.Media;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Eto.Drawing;
using Eto.Forms;

namespace Cavra_Control
{
    class MacroFunctionality
    {
        //TextBox userinput_txt;
        string macroData;
        string fileName;
        string FULL_fileName;

        MenuBar menu;
        Button OK_btn;
        Dialog dialog;
        

        public Dialog AskUserForMacroName_Dialog()
        {
            dialog = new Dialog();
            dialog.ClientSize= new Size(600, 400);
            dialog.WindowState = WindowState.Normal;

            var layout = new DynamicLayout(dialog);
            
            var instructions_lbl = new Label();
            
            instructions_lbl.Text = "Enter the Desired Macro Name and Information Below. NOTE: Existing Macros with the Same Name will be Replaced.";
            
            userinput_MacroName_txt = new TextBox();
            userinput_MacroName_txt.PlaceholderText = "Enter Name of Macro.";
            userinput_RightSlider_txt = new TextBox();
            userinput_RightSlider_txt.PlaceholderText = "Enter Right Slider Value.";
            userinput_LeftSlider_txt = new TextBox();
            userinput_LeftSlider_txt.PlaceholderText = "Enter Left Slider Value.";
            
            //Create a string that will contain data representing above values.

            OK_btn = new Button();
            OK_btn.Text = "OK";
            
            layout.Add(instructions_lbl);
            layout.Add(userinput_MacroName_txt);
            layout.AddRow(userinput_RightSlider_txt);
            layout.AddRow(userinput_LeftSlider_txt);
            layout.AddCentered(OK_btn);

            OK_btn.Click += ChangeDialogResult;

            return dialog;
        }

        public void ChangeDialogResult(object sender, EventArgs e)
        {
            dialog.DialogResult = DialogResult.Ok;
            dialog.Close();
        }

        public void CreateNewMacro()
        {
            macroData = "Macro Name," + userinput_MacroName_txt.Text + ".Right Slider." + userinput_RightSlider_txt.Text + "#Left Slider#" + userinput_LeftSlider_txt.Text; 

            fileName = userinput_MacroName_txt.Text;
            
            FULL_fileName = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName + ".txt");
            
            //if (Directory.GetCurrentDirectory().Contains(FULL_fileName) == false)
           
            File.WriteAllText(FULL_fileName, macroData);
        }

        public void GenerateMacroButton(ImageMenuItem buttonOnMenu)
        {
            int numberOfMenuItems = 0;

            var GeneratedMacroButton = new ImageMenuItem();
            GeneratedMacroButton.Text = fileName;

            numberOfMenuItems++;

            if (numberOfMenuItems >= 7)
            {
                menu.MenuItems.RemoveAt(7);
                numberOfMenuItems--;
            }

            buttonOnMenu.MenuItems.Add(GeneratedMacroButton);

            GeneratedMacroButton.Click += delegate
            {
                StreamReader reader = new StreamReader(FULL_fileName);
                char[] delimiterCharacters = { ',','.','#' };
                string content = reader.ReadToEnd();
                string[] words = content.Split(delimiterCharacters);

                string macro_name_data = words[1];

                string rightsliderdata = words[3];

                string leftsliderdata = words[5];
                reader.Close();
                
                //need to have a GeneratedMenuButton load rightslider/leftslider values properly - polymorphism/inheritance issue (can't access rightslider.value)
                //have application check default macrobutton save directory on initialization and automatically load the text files' data so they are prepped to access even after program is reopened.
                
                
                
            };
        }



        public TextBox userinput_RightSlider_txt { get; set; }

        public TextBox userinput_LeftSlider_txt { get; set; }
    
        public  TextBox userinput_MacroName_txt { get; set; }
    }
}
