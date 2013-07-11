﻿using System;
using System.IO;
using System.Text;
using System.Media;

using Eto.Drawing;
using Eto.Forms;

using NCA.CavraDriver;

namespace Cavra_Control
{
    class MainClass : Application
    {


        [STAThread]
        public static void Main(string[] args)
        {

            var app = new Application();

            app.Initialized += delegate
            {
                app.MainForm = new CavraControl();
                app.MainForm.Show();
            };
            app.Run(args);

        }



        class CavraControl : Form
        {
			readonly Bitmap RIGHT_MUTE_IMG = LoadResource("Cavra_Control.rightmute.png");
        	readonly Bitmap RIGHT_SOUND_MEDIUM_MON_IMG = LoadResource("Cavra_Control.rightsoundmediumon.png");
	        readonly Bitmap RIGHT_SOUND_ON_IMG = LoadResource("Cavra_Control.rightsoundon.png");
	        readonly Bitmap LEFT_MUTE_IMG = LoadResource("Cavra_Control.leftmute.png");
	        readonly Bitmap LEFT_SOUND_MEDIUM_MON_IMG = LoadResource("Cavra_Control.leftsoundmediumon.png");
	        readonly Bitmap LEFT_SOUND_ON_IMG = LoadResource("Cavra_Control.leftsoundon.png");
	        readonly Bitmap PLAY_ICON = LoadResource("Cavra_Control.playicon.png");
	        readonly Bitmap STOP_ICON = LoadResource("Cavra_Control.stopicon.png");

            int screenX;
            int screenY;

            Slider rightSlider;
            Slider leftSlider;

            TextBox rightSlidertxtbox;
            TextBox leftSlidertxtbox;
            TextBox waveFiletxtbox;

            Button rightMutebtn;
            Button leftMutebtn;

            SoundPlayer player = null;
            string wavefilestring;

            int rightSliderValueSaved;
            int leftSliderValueSaved;

            ImageMenuItem Macro_btn;

            MacroFunctionality macrofunctionality;

            MenuBar menu;

            Cavra cavra;

            public CavraControl()
            {
                this.ClientSize = new Size(400, 350);
                this.Title = "Cavra Control";
#if WINDOWS
                this.WindowState = WindowState.Normal;
				this.BringToFront();
#endif

                cavra = new Cavra();
                cavra.Connect();

                GenerateMenu();

                /*
 *
                Size defaultSize = new Size(this.Screen.Bounds.Width - 50, this.Screen.Bounds.Height - 50);
                this.Size = defaultSize;
                int screenX = (int)(this.Screen.Bounds.X - 100);
                int screenY = (int)(this.Screen.Bounds.X - 100);
                Eto.Drawing.Point point = new Point(this.Screen.Bounds.X - 100, this.Screen.Bounds.Y - 100);
*/
                //try to get window to appear centered at all times.

                //this.Load += OnFormLoaded;

                /*
                this.Shown += OnFormShown;
                this.Show();
                */
                FormLayoutEstablish();

            }
#if CENTERED_WINDOWS_FEATURE
            //wip
            void OnFormLoaded(object s, EventArgs e)
            {
                screenX = (int)(this.Screen.Bounds.X - 2000);
                screenY = (int)(this.Screen.Bounds.Y - 1);
            }

            location not finished-, use .PrimaryScreen() instead

            void OnFormShown(object s, EventArgs e)
            {
                this.Location = (Point)(this.Screen.Bounds.Center - (this.Size / 2));
            }

#endif
			static Bitmap LoadResource(string resource)
        	{
            	return new Bitmap(Bitmap.FromResource(resource), 16, 16);
        	}

            void GenerateMenu()
            {
                menu = new MenuBar();

                Macro_btn = new ImageMenuItem { Text = "&Macros" };

                var Create_New_btn = new ImageMenuItem();
                Create_New_btn.Text = "Create New";

                Macro_btn.MenuItems.Add(Create_New_btn);

                menu.MenuItems.Add(Macro_btn);

                var recentlyopened_btn = new ImageMenuItem { Text = "&Recently Opened" };
                menu.MenuItems.Add(recentlyopened_btn);



                this.Menu = menu;

                Create_New_btn.Click += Macro_btn_Clicked;
            }

            void FormLayoutEstablish()
            {
                var root_panel = new DynamicLayout(this);

                root_panel.Add(PlayerControlBuilder());
                root_panel.Add(VolumeControlBuilder());
                root_panel.Add(new Panel());
            }

            Control PlayerControlBuilder()
            {
                var layout = new DynamicLayout(new Panel());
                layout.Add(new Label
                {
                    Text = "Cavra Control",
                    Font = new Font(SystemFont.Bold, 17),
                    HorizontalAlign = HorizontalAlign.Left
                });

                layout.Add(new Label
                {
                    Text = "NCA",
                    Font = new Font(SystemFont.Default, 13),
                    HorizontalAlign = HorizontalAlign.Left
                });

                Label wavefile = new Label
                {
                    Text = "WaveFile",
                    TextColor = Colors.Blue,
                    HorizontalAlign = HorizontalAlign.Left
                };

                waveFiletxtbox = new TextBox();
                waveFiletxtbox.PlaceholderText = "Open a Wave File";
                wavefilestring = waveFiletxtbox.Text;
                waveFiletxtbox.TextChanged += TextChangeInWaveFileTxtBox;


                Button open = new Button
                {
                    Text = "Browse"
                };
                open.Click += OpenFileDialogWindow;

                var file_panel = new DynamicLayout(new Panel());
                file_panel.BeginHorizontal();
                file_panel.Add(wavefile, false, false);
                file_panel.Add(waveFiletxtbox, true, false);
                file_panel.Add(open, false, false);
                file_panel.EndHorizontal();
                layout.Add(file_panel.Container);

                Button play = new Button();
                play.Text = "Play";
                play.Image = PLAY_ICON;
                play.Click += delegate { if (null != player)
                    try { player.Play(); }
                    catch (Exception exc)
                    {
                    }
                    };

                Button stop = new Button();
                stop.Text = "Stop";
                stop.Image = STOP_ICON;
                stop.Click += delegate { if (null != player) player.Stop(); };

                var btn_panel = new DynamicLayout(new Panel());
                btn_panel.AddRow(null, play, stop);

                layout.Add(btn_panel.Container, false, false);

                return layout.Container;
            }

            Control VolumeControlBuilder()
            {
                var layout = new DynamicLayout(new Panel());

                rightSlider = new Slider();

                rightSlider.Value = 50;
                rightSlider.MaxValue = 100;
                rightSlider.MinValue = 0;
                rightSlider.ValueChanged += rightSliderChanged;

                rightSlidertxtbox = new TextBox();
                rightSlidertxtbox.Font = new Font(SystemFont.Default, 8);
                int rightValue = 50;
                rightSlidertxtbox.Text = rightValue.ToString();
                rightSlidertxtbox.MaxLength = 3;
                //rightSlidertxtbox.TextChanged += RightSlidertxtboxSizeChanging;
                rightSlidertxtbox.TextChanged += rightSliderTxtBoxChanged;

                rightMutebtn = new Button();
                rightMutebtn.Image = RIGHT_SOUND_ON_IMG;
                rightMutebtn.BackgroundColor = Colors.LightSkyBlue;
                rightMutebtn.ImagePosition = ButtonImagePosition.Overlay;
                rightMutebtn.Click += MuteRightSlider;

                /* if you want no borders around mute button.
                ImageView image = new ImageView();
                image.Image = new Bitmap(Eto.EtoEnvironment.GetFolderPath(Eto.EtoSpecialFolder.ApplicationResources) + "\\rightmute.png");
                image.MouseDown += MuteRightSlider;
                 */

                //
                Label rightSliderlbl = new Label();
                rightSliderlbl.Text = "Right Slider";
                rightSliderlbl.HorizontalAlign = HorizontalAlign.Center;

                layout.BeginHorizontal();
                layout.Add(rightSlider, true, false);
                layout.Add(rightSlidertxtbox, false, false);
                layout.Add(rightMutebtn, false, false);
                //layout.Add(image, false, false);

                //layout.AddRow(rightSliderlbl, rightSlider, rightSlidertxtbox, rightMutebtn);

                leftSlider = new Slider();
                leftSlider.Value = 50;
                leftSlider.MaxValue = 100;
                leftSlider.MinValue = 0;
                leftSlider.ValueChanged += leftSliderChanged;

                leftSlidertxtbox = new TextBox();
                leftSlidertxtbox.Font = new Font(SystemFont.Default, 8);
                int leftValue = 50;
                leftSlidertxtbox.Text = (string)leftValue.ToString();
                leftSlidertxtbox.MaxLength = 3;
                leftSlidertxtbox.TextChanged += leftSliderTxtBoxChanged;

                leftMutebtn = new Button();
                leftMutebtn.Image = LEFT_SOUND_ON_IMG;
                leftMutebtn.ImagePosition = ButtonImagePosition.Overlay;
                leftMutebtn.BackgroundColor = Colors.OrangeRed;
                leftMutebtn.Click += MuteLeftSlider;
                layout.BeginHorizontal();
                layout.Add(leftSlider, true, false);
                layout.Add(leftSlidertxtbox, false, false);
                layout.Add(leftMutebtn, false, false);
                return layout.Container;
            }


            void OpenFileDialogWindow(object sender, EventArgs e)
            {
                OpenFileDialog openfiledialog = new OpenFileDialog();

                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Uri openDirectory = new Uri(folderPath);
                openfiledialog.Directory = openDirectory;

                openfiledialog.Filters = new FileDialogFilter[] { new FileDialogFilter("WAV", ".wav")};



                if (openfiledialog.ShowDialog(ParentWindow) == DialogResult.Ok)
                {
                    waveFiletxtbox.Text = openfiledialog.FileName;
                    player = new SoundPlayer(waveFiletxtbox.Text);
                    /*
                    if (null != player)
                        player.Stop();
                    */

                }

            }

            void TextChangeInWaveFileTxtBox(object sender, EventArgs e)
            {
                try
                {
                    player = new SoundPlayer(waveFiletxtbox.Text);
                }
                catch (Exception ex)
                {
                    player = null;
                }
            }

            void rightSliderChanged(object sender, EventArgs e)
            {
                if (rightSlider.Value == 100)
                {
                    rightMutebtn.Image = RIGHT_MUTE_IMG;
                }
                else if (rightSlider.Value >= 50)
                {
                    rightMutebtn.Image = RIGHT_SOUND_MEDIUM_MON_IMG;
                }
                else
                {
                    rightMutebtn.Image = RIGHT_SOUND_ON_IMG;
                }
                rightSlidertxtbox.Text = rightSlider.Value.ToString();
            }

            void leftSliderChanged(object sender, EventArgs e)
            {
                if (leftSlider.Value == 100)
                {
                    leftMutebtn.Image = LEFT_MUTE_IMG;
                }
                else if (leftSlider.Value >= 50)
                {
                    leftMutebtn.Image = LEFT_SOUND_MEDIUM_MON_IMG;
                }
                else
                {
                    leftMutebtn.Image = LEFT_SOUND_ON_IMG;
                }
                leftSlidertxtbox.Text = leftSlider.Value.ToString();
            }

            void rightSliderTxtBoxChanged(object sender, EventArgs e)
            {
                try
                {
                    int value1 = Convert.ToInt32(rightSlidertxtbox.Text);
                    rightSlider.Value = value1;
                    cavra.Attenuator.Right = value1;

                } catch (Exception) {
                    rightSlider.Value = 0;
                }

            }

            void leftSliderTxtBoxChanged(object sender, EventArgs e)
            {
                int value2;
                try
                {
                    value2 = Convert.ToInt32(leftSlidertxtbox.Text);
                    leftSlider.Value = value2;

                    cavra.Attenuator.Left = value2;

                } catch (Exception) {
                    leftSlider.Value = 0;
                }

            }

            void MuteRightSlider(object sender, EventArgs e)
            {
                if (rightSlider.Value != 100)
                {
                    rightSliderValueSaved = rightSlider.Value;
                    rightSlider.Value = 100;
                    rightSlidertxtbox.Text = "100";
                }
                else
                {
                    rightSlider.Value = rightSliderValueSaved;
                }
            }

            void MuteLeftSlider(object sender, EventArgs e)
            {
                if (leftSlider.Value != 100)
                {
                    leftSliderValueSaved = leftSlider.Value;
                    leftSlider.Value = 100;
                    leftSlidertxtbox.Text = "100";
                }
                else
                {
                    leftSlider.Value = leftSliderValueSaved;
                }
            }

            void Macro_btn_Clicked(object sender, EventArgs e)
            {

                macrofunctionality = new MacroFunctionality();

                if (macrofunctionality.AskUserForMacroName_Dialog().ShowDialog(this) == DialogResult.Ok)
                {
                    Dialog_Button_OK_Clicked();
                }


            }

            void Dialog_Button_OK_Clicked()
            {
                macrofunctionality.CreateNewMacro();
                macrofunctionality.GenerateMacroButton(Macro_btn);

            }

            public void GeneratedMacroButtonClicked(object sender, EventArgs e)
            {

            }
/* Text Dialog that changes size is WIP.
            void RightSlidertxtboxSizeChanging(object sender, EventArgs e)
            {
                string str = rightSlidertxtbox.Text;
                Font font = new Font(SystemFont.Default, 17);
                rightSlidertxtbox.Size = new Size(MeasureTextSize(font, str));
                //rightSlidertxtbox.Size = new Size(rightSlidertxtbox.Text.Length*13, 25);

                //find a way to measure text size and adjust textbox based off of that.
            }

            public SizeF MeasureTextSize(Font font, string str)
            {
                Graphics graphics = new Graphics(new Bitmap(12, 12, PixelFormat.Format32bppRgb));
                //get rid of micrsoft references at top.
                return graphics.MeasureString(font, str);
            }
 * */
        }
    }
}
