using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace giaodien
{
    public partial class Form1 : Form
    {
        PictureBox pbBasket = new PictureBox();
        PictureBox pbDuck = new PictureBox();
        Timer tmBasket = new Timer();
        Timer tmSpawnEgg = new Timer();
        Timer tmDuck = new Timer();
        Timer tmTime = new Timer();
        List<PictureBox> eggs = new List<PictureBox>();
        Label lblTime = new Label();
        Label lblEggCount = new Label();
        Button btnStartGame = new Button(); // Nút Bắt đầu trò chơi

        Random random = new Random();
        SoundPlayer backgroundMusic;

        int elapsedSeconds = 0;
        bool moveLeft = false;
        bool moveRight = false;
        int xBasket = 300;
        int yBasket = 300;
        int xDeltaBasket = 30;

        int xDuck = 300;
        int yDuck = 40;
        int xDeltaDuck = 5;
        int duckSpeedIncreaseInterval = 5;
        int duckSpeedIncreaseAmount = 1;

        int eggCount = 0;
        int eggFallSpeed = 10;
        int spawnEggInterval = 2500;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // Kích hoạt double buffering
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Thiết lập Background Image cho Form
            this.BackgroundImage = Image.FromFile("../../images/background1.jpg");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Thiết lập nút Bắt đầu
            btnStartGame.Text = "Bắt đầu";
            btnStartGame.Font = new Font("Arial", 16, FontStyle.Bold);
            btnStartGame.Size = new Size(150, 50);
            btnStartGame.Location = new Point((this.ClientSize.Width - btnStartGame.Width) / 2,
                                              (this.ClientSize.Height - btnStartGame.Height) / 2);
            btnStartGame.Click += BtnStartGame_Click; // Thêm sự kiện nhấn nút
            this.Controls.Add(btnStartGame);

            this.KeyPreview = true;

            // Thiết lập các thành phần khác
            InitializeGameComponents();
        }

        private void InitializeGameComponents()
        {
            // Thiết lập và phát nhạc nền
            backgroundMusic = new SoundPlayer("../../sounds/soundbackground1.wav");

            // Timer để di chuyển giỏ liên tục
            tmBasket.Interval = 10;
            tmBasket.Tick += tmBasket_Tick;
            tmBasket.Start();

            // Thiết lập Timer để tạo trứng mới với thời gian khởi tạo
            tmSpawnEgg.Interval = spawnEggInterval;
            tmSpawnEgg.Tick += tmSpawnEgg_Tick;

            // Timer để di chuyển vịt
            tmDuck.Interval = 30;
            tmDuck.Tick += tmDuck_Tick;

            // Cấu hình cho giỏ
            pbBasket.SizeMode = PictureBoxSizeMode.StretchImage;
            pbBasket.Size = new Size(70, 70);
            pbBasket.Location = new Point(xBasket, yBasket);
            pbBasket.BackColor = Color.Transparent;
            pbBasket.Image = Image.FromFile("../../images/Basket.png");
            this.Controls.Add(pbBasket);

            // Cấu hình cho vịt
            pbDuck.SizeMode = PictureBoxSizeMode.StretchImage;
            pbDuck.Size = new Size(100, 100);
            pbDuck.Location = new Point(xDuck, yDuck);
            pbDuck.BackColor = Color.Transparent;
            pbDuck.Image = Image.FromFile("../../images/Duck.png");
            this.Controls.Add(pbDuck);

            // Gán sự kiện cho phím bấm
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.KeyUp += new KeyEventHandler(Form1_KeyUp);

            // Thiết lập Label để hiển thị thời gian
            lblTime.Font = new Font("Arial", 16, FontStyle.Bold);
            lblTime.ForeColor = Color.Black;
            lblTime.BackColor = Color.Transparent;
            lblTime.Location = new Point(10, 10);
            lblTime.Size = new Size(200, 50);
            lblTime.Text = "Time: 00:00:00";
            lblTime.BringToFront();
            this.Controls.Add(lblTime);

            // Thiết lập Timer để cập nhật thời gian
            tmTime.Interval = 1000; // 1 giây
            tmTime.Tick += tmTime_Tick;

            // Thiết lập Label để hiển thị số lượng trứng
            lblEggCount.Font = new Font("Arial", 16, FontStyle.Bold);
            lblEggCount.ForeColor = Color.Black;
            lblEggCount.BackColor = Color.Transparent;
            lblEggCount.Location = new Point(this.ClientSize.Width - 150, 10);
            lblEggCount.Size = new Size(140, 50);
            lblEggCount.Text = "Eggs: 0";
            lblEggCount.BringToFront();
            this.Controls.Add(lblEggCount);
        }

        private void BtnStartGame_Click(object sender, EventArgs e)
        {
            // Bắt đầu trò chơi khi nhấn nút "Bắt đầu"
            btnStartGame.Visible = false; // Ẩn nút bắt đầu

            // Bắt đầu các Timer và nhạc nền
            tmBasket.Start();
            tmSpawnEgg.Start();
            tmDuck.Start();
            tmTime.Start();
            backgroundMusic.PlayLooping();
        }

        private void tmDuck_Tick(object sender, EventArgs e)
        {
            if (elapsedSeconds % duckSpeedIncreaseInterval == 0 && elapsedSeconds > 0)
            {
                xDeltaDuck += duckSpeedIncreaseAmount;
            }

            xDuck += xDeltaDuck;

            if (xDuck >= this.ClientSize.Width - pbDuck.Width)
            {
                xDuck = this.ClientSize.Width - pbDuck.Width;
                xDeltaDuck = -Math.Abs(xDeltaDuck);
            }
            else if (xDuck <= 0)
            {
                xDuck = 0;
                xDeltaDuck = Math.Abs(xDeltaDuck);
            }

            pbDuck.Location = new Point(xDuck, yDuck);
        }

        private void tmSpawnEgg_Tick(object sender, EventArgs e)
        {
            PictureBox newEgg = new PictureBox();
            newEgg.SizeMode = PictureBoxSizeMode.StretchImage;
            newEgg.Size = new Size(50, 50);
            newEgg.BackColor = Color.Transparent;
            newEgg.Image = Image.FromFile("../../images/Egg.png");

            int newEggX = pbDuck.Location.X + pbDuck.Width / 2 - newEgg.Width / 2;
            int newEggY = pbDuck.Location.Y + pbDuck.Height;

            newEgg.Location = new Point(newEggX, newEggY);
            this.Controls.Add(newEgg);
            eggs.Add(newEgg);

            Timer newEggTimer = new Timer();
            newEggTimer.Interval = 50;
            newEggTimer.Tick += (s, ev) => MoveEgg(newEgg, newEggTimer);
            newEggTimer.Start();
        }

        private void MoveEgg(PictureBox egg, Timer eggTimer)
        {
            eggFallSpeed = 10 + (elapsedSeconds / 8);

            int yEgg = egg.Location.Y + eggFallSpeed;

            egg.Location = new Point(egg.Location.X, yEgg);

            if (pbBasket.Bounds.IntersectsWith(egg.Bounds))
            {
                eggTimer.Stop();
                this.Controls.Remove(egg);
                egg.Dispose();

                eggCount++;
                lblEggCount.Text = "Eggs: " + eggCount;

                if (eggCount == 16)
                {
                    this.BackgroundImage = Image.FromFile("../../images/background2.jpg");
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else if (eggCount == 20)
                {
                    this.BackgroundImage = Image.FromFile("../../images/background3.jpg");
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else if (eggCount == 30)
                {
                    GameWin();
                    return;
                }

                return;
            }

            if (yEgg >= this.ClientSize.Height - egg.Height)
            {
                egg.Image = Image.FromFile("../../images/Egg_broken.png");
                eggTimer.Stop();
                System.Threading.Thread.Sleep(2000);
                this.Controls.Remove(egg);
                egg.Dispose();
                GameOver();
            }
        }

        private void GameOver()
        {
            tmBasket.Stop();
            tmSpawnEgg.Stop();
            tmDuck.Stop();
            tmTime.Stop();
            backgroundMusic.Stop();

            MessageBox.Show("Game kết thúc! Trứng đã vỡ!", "Kết thúc trò chơi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Application.Restart();
        }

        private void GameWin()
        {
            tmBasket.Stop();
            tmSpawnEgg.Stop();
            tmDuck.Stop();
            tmTime.Stop();
            backgroundMusic.Stop();

            MessageBox.Show("Giỏ đã đầy trứng! Bạn đã thắng!", "You Win", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Application.Restart();
        }

        private void tmBasket_Tick(object sender, EventArgs e)
        {
            if (moveLeft && xBasket > 0)
            {
                xBasket -= xDeltaBasket;
            }
            if (moveRight && xBasket < this.ClientSize.Width - pbBasket.Width)
            {
                xBasket += xDeltaBasket;
            }

            pbBasket.Location = new Point(xBasket, yBasket);
        }

        private void tmTime_Tick(object sender, EventArgs e)
        {
            elapsedSeconds++;
            lblTime.Text = "Time: " + TimeSpan.FromSeconds(elapsedSeconds).ToString(@"hh\:mm\:ss");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                moveLeft = true;
            }
            else if (e.KeyCode == Keys.Right)
            {
                moveRight = true;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                moveLeft = false;
            }
            else if (e.KeyCode == Keys.Right)
            {
                moveRight = false;
            }
        }
    }
}