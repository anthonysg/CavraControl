using System;
using System.IO;
using System.Text;
using System.Media;

using Eto.Drawing;
using Eto.Forms;

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

            public CavraControl()
            {
                this.ClientSize = new Size(400, 350);
                this.WindowState = WindowState.Normal;
                this.Title = "Cavra Control";
                this.BringToFront();
                //Size defaultSize = new Size(this.Screen.Bounds.Width - 50, this.Screen.Bounds.Height - 50);
                //this.Size = defaultSize;
                //int screenX = (int)(this.Screen.Bounds.X - 100);
                //int screenY = (int)(this.Screen.Bounds.X - 100);
                //Eto.Drawing.Point point = new Point(this.Screen.Bounds.X - 100, this.Screen.Bounds.Y - 100);
                
                //try to get window to appear centered at all times.
                

                //this.Load += OnFormLoaded;
                /*
                this.Shown += OnFormShown;
                this.Show();
                */
                FormLayoutEstablish();
            }

            //void OnFormLoaded(object s, EventArgs e)
            //{
                //screenX = (int)(this.Screen.Bounds.X - 2000);
                //screenY = (int)(this.Screen.Bounds.Y - 1);
            //}
            
            //location not finished-, use .PrimaryScreen() instead
            /*
            void OnFormShown(object s, EventArgs e)
            {
                this.Location = (Point)(this.Screen.Bounds.Center - (this.Size / 2));
            }
            */
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

                waveFiletxtbox = new TextBox
                {
                    PlaceholderText = "Open a Wave File"
                };

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
                play.Image = new Bitmap(Eto.EtoEnvironment.GetFolderPath(Eto.EtoSpecialFolder.ApplicationResources) + "\\playicon.png");
                play.ImagePosition = ButtonImagePosition.Overlay;
                play.Click += delegate { if (null != player) player.Play(); };
                
                Button stop = new Button();
                stop.Text = "STOP";
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
                rightMutebtn.Text = "Mute";
                rightMutebtn.Click += MuteRightSlider;
                //
                Label rightSliderlbl = new Label();
                rightSliderlbl.Text = "Right Slider";
                rightSliderlbl.HorizontalAlign = HorizontalAlign.Center;

                layout.BeginHorizontal();
                layout.Add(rightSlider, true, false);
                layout.Add(rightSlidertxtbox, false, false);
                layout.Add(rightMutebtn, false, false);
                
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
                leftMutebtn.Text = "Mute";
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
                    /*
                    if (null != player)
                        player.Stop();

                    player = new SoundPlayer(openfiledialog.FileName);
                     */
                }            
                
            }

            void rightSliderChanged(object sender, EventArgs e)
            {
                rightSlidertxtbox.Text = rightSlider.Value.ToString();
            }

            void leftSliderChanged(object sender, EventArgs e)
            {
                leftSlidertxtbox.Text = leftSlider.Value.ToString();
            }

            void rightSliderTxtBoxChanged(object sender, EventArgs e)
            {
                try
                {
                    int value1 = Convert.ToInt32(rightSlidertxtbox.Text);
                    rightSlider.Value = value1;
                }
                
                catch (Exception) 
                {
                    int value1 = 0;
                    rightSlider.Value = value1;
                }
            }

            void leftSliderTxtBoxChanged(object sender, EventArgs e)
            {
                try
                {
                    int value2 = Convert.ToInt32(leftSlidertxtbox.Text);
                    leftSlider.Value = value2;
                }

                catch (Exception)
                {
                    int value2 = 0;
                    leftSlider.Value = value2;
                }
            }
            void MuteRightSlider(object sender, EventArgs e)
            {
                rightSlider.Value = 100;
                rightSlidertxtbox.Text = "100";
            }

            void MuteLeftSlider(object sender, EventArgs e)
            {
                leftSlider.Value = 100;
                rightSlidertxtbox.Text = "100";
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
