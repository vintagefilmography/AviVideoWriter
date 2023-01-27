using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;
using AForge.Video.VFW;


namespace CAMERAPROJECT
{
    public partial class Form1 : Form
    {
        public int islemdurumu = 0; //CAMERA STATUS
        FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        VideoCaptureDevice videoSource = null;
        public static int durdur = 0;
        public static int gondermesayisi = 0;
        public int kamerabaslat = 0;
        public int selected = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void START_Click(object sender, EventArgs e)
        {
            selected = comboBox1.SelectedIndex;

            if (islemdurumu == 0)
            {


                if (kamerabaslat > 0) return;
                try
                {
                    //videoSource = new VideoCaptureDevice(videoDevices[selected].MonikerString);
                    const int IDX = 0; // change this
                    videoSource = new VideoCaptureDevice(videoDevices[selected].MonikerString)
                     {
                        VideoResolution = new VideoCaptureDevice(videoDevices[selected].MonikerString).VideoCapabilities[IDX]
                    };
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame); 
                    videoSource.Start(); kamerabaslat = 1; //CAMERA STARTRED
                    videoSource.DesiredFrameRate = 10;

                }
                catch
                {
                    MessageBox.Show("RESTART THE PROGRAM");

                    if (!(videoSource == null))
                        if (videoSource.IsRunning)
                        {
                            videoSource.SignalToStop();
                            videoSource = null;
                        }
                }//catch
            }
        }////
        int newFrameCount = 0;
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            newFrameCount++;
            if (newFrameCount == 6)
            {
                newFrameCount = 1;
                Bitmap img = (Bitmap)eventArgs.Frame.Clone();
                imgVideo.Image = img;
            }
        }

        private void RESET_Click(object sender, EventArgs e)
        {
            if (!(videoSource == null))
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }
             
            kamerabaslat = 0;
            imgVideo.Image = null;

            label1.Text = "CAMERA TURN OFF";
        }
        

  
        int CAPTUREtoggle = 0;
        private void CAPTURE_Click(object sender, EventArgs e)
        {

            if (CAPTUREtoggle == 0)
            {
                CAPTURE.BackColor = Color.Red;
                CAPTUREtoggle = 1;
            }
            else
            {
                CAPTURE.BackColor = Color.White;
                CAPTUREtoggle = 0;
            }
 

                //Image display in 2nd vindow for testing (Old capture button)
                //if (imgVideo != null) { imgCapture.Image = imgVideo.Image; }

          
        }

        private void PAUSE_Click(object sender, EventArgs e)
        {
           
                if (!(videoSource == null))
                    if (videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();
                        videoSource = null;
                    }

            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
         

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.label1.Text = "";
                //Enumerate all video input devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    label1.Text = "No local capture devices";
                }
                foreach (FilterInfo device in videoDevices)
                {
                    int i = 1;
                    comboBox1.Items.Add(device.Name);
                    label1.Text = ("camera" + i + "initialization completed..." + "\n");
                    i++;
                }
                comboBox1.SelectedIndex = 0;
            }
            catch (ApplicationException)
            {
                this.label1.Text = "No local capture devices";
                videoDevices = null;
            }

        }

        private void EXIT_Click(object sender, EventArgs e)
        {
            try
            {
                videoSource.SignalToStop();
                videoSource = null;
                if (!(videoSource == null))
                {
                    videoSource.Stop();
                    videoSource = null;
                }
            }
            catch { }
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                videoSource.SignalToStop();
                videoSource = null;
                if (!(videoSource == null))
                {
                    videoSource.Stop();
                    videoSource = null;
                }
            }
            catch { }
            Application.Exit();

        }


        // instantiate AVI writer, use WMV3 codec
        //AVIWriter writer = new AVIWriter("wmv3");
        AVIWriter writer = new AVIWriter();
        int imageNumber = 0;
        private void SAVE_Click(object sender, EventArgs e)
        {
            int width = 1280;
            int height = 720;
            Int32 frameRate = 25;
            if (comboBox2.SelectedIndex != (-1))
            {
                frameRate = Int32.Parse(comboBox2.SelectedItem.ToString());
            }

            if (comboBox3.SelectedIndex != (-1))
            {


                if (comboBox3.SelectedIndex == 0)
                {
                    width = 160;
                    height = 120;
                }
                if (comboBox3.SelectedIndex == 1)
                {
                    width = 176;
                    height = 144;
                }
                if (comboBox3.SelectedIndex == 2)
                {
                    width = 320;
                    height = 240;
                }
                if (comboBox3.SelectedIndex == 3)
                {
                    width = 352;
                    height = 288;
                }
                if (comboBox3.SelectedIndex == 4)
                {
                    width = 640;
                    height = 480;
                }
                if (comboBox3.SelectedIndex == 5)
                {
                    width = 1280;
                    height = 720;
                }

                if (comboBox3.SelectedIndex == 6)
                {
                    width = 1920;
                    height = 1080;
                }
            }

            try
            {
                imgCapture.Image = imgVideo.Image;
            }
            catch { }
            /* kaydet butonu  bntSave_Click*/


            if (CAPTUREtoggle == 1)
            {
                // create instance of video writer
                imageNumber++;
                if (imageNumber == 1)
                {
                    // create new AVI file and open it
                    writer.Open("C:\\capture\\test.avi", width, height);
                    writer.FrameRate = frameRate;
                }
                else
                {
                    try
                    {
                        imgCapture.Image = imgVideo.Image;
                        Bitmap imageFrame = imgVideo.Image as Bitmap;
                        //var reduced = new Bitmap(width, height);
                        //imageFrame.Save("mytest.bmp");
                        writer.AddFrame(imageFrame);
                        //label1.Text = "IMAGE SAVED";
                    }
                    catch { }
                }
                if ((CAPTUREtoggle == 0) && (imageNumber > 1))
                    writer.Close();
            }
            else
            {
                try
                {
                    imageNumber++;
                    imgCapture.Image.Save(@"C:\capture\" + imageNumber + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                    label1.Text = "IMAGE SAVED";

                }
                catch { }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        ///
    }
}
