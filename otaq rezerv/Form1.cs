using System;
using System.Drawing;
using System.Windows.Forms;

namespace otaq_rezerv
{
    public partial class Form1 : Form
    {
        // 'CS0104' hatasını çözmek için tam ad alanını (System.Windows.Forms.Timer) belirtiyoruz
        private readonly System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();

        // 6 oda için kalan saniye (Örn: 5 dk = 300 saniye)
        private readonly int[] roomSeconds = new int[6];

        // 6 oda için rezervasyon yapan kişilerin isimleri
        private readonly string[] roomUsers = new string[6];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            // Zamanlayıcı (Timer) ayarları
            timer1.Interval = 1000; // 1 saniye (1000 ms)
            timer1.Tick += timer1_Tick;
            timer1.Start();

            // Oda butonlarının tıklama olaylarını bağlama
            button1.Click += OtaqButton_Click;
            button2.Click += OtaqButton_Click;
            button3.Click += OtaqButton_Click;
            button4.Click += OtaqButton_Click;
            button5.Click += OtaqButton_Click;
            button6.Click += OtaqButton_Click;
        }

        // Oda butonlarına tıklandığında çalışacak metot
        private void OtaqButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;

            // Tıklanan butonun adından oda indeksini belirleme (button1 -> index 0)
            int index = Convert.ToInt32(btn.Name.Replace("button", "")) - 1;

            // Oda dolu ise bilgilendirme
            if (roomSeconds[index] > 0)
            {
                MessageBox.Show($"Bu otaq {roomUsers[index]} tərəfindən rezerv edilib!\nQalan vaxt: {GetFormattedTime(roomSeconds[index])}",
                                "Otaq Doludur", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Metin kutularından veri alma
            string adSoyad = textBox1.Text.Trim();
            string muddetText = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(adSoyad))
            {
                MessageBox.Show("Zəhmət olmasa Ad və Soyadı daxil edin!", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(muddetText, out int deqiqe) || deqiqe <= 0)
            {
                MessageBox.Show("Zəhmət olmasa müddəti (dəqiqə ilə) düzgün rəqəm kimi daxil edin!", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Rezervasyonu kaydetme
            roomSeconds[index] = deqiqe * 60; // Dakikayı saniyeye çevirme
            roomUsers[index] = adSoyad;

            // Buton rengini güncelleme
            btn.BackColor = Color.LightGreen;

            // Metin kutularını temizleme
            textBox1.Clear();
            textBox2.Clear();

            // Etiket güncelleme
            UpdateTimerLabel(index);
        }

        // Zamanlayıcı her saniye çalıştığında
        private void timer1_Tick(object? sender, EventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                if (roomSeconds[i] > 0)
                {
                    roomSeconds[i]--; // Saniyeyi 1 azaltma
                    UpdateTimerLabel(i);

                    // Süre bittiğinde
                    if (roomSeconds[i] == 0)
                    {
                        ResetRoom(i);
                        MessageBox.Show($"Otaq {i + 1}-in rezerv vaxtı bitti!", "Məlumat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        // Saniyeyi metin olarak güncelleme
        private void UpdateTimerLabel(int index)
        {
            Label? lbl = GetLabelByIndex(index);
            if (lbl != null)
            {
                lbl.Text = GetFormattedTime(roomSeconds[index]);
            }
        }

        // Dakika:Saniye formatlama
        private static string GetFormattedTime(int totalSeconds)
        {
            int min = totalSeconds / 60;
            int sec = totalSeconds % 60;
            return $"{min:D2}:{sec:D2}";
        }

        // Odayı ilk haline getirme
        private void ResetRoom(int index)
        {
            Button? btn = GetButtonByIndex(index);
            Label? lbl = GetLabelByIndex(index);

            if (btn != null) btn.BackColor = Color.White;
            if (lbl != null) lbl.Text = "0";

            roomUsers[index] = string.Empty;
        }

        // İndekse göre Buton getirme
        private Button? GetButtonByIndex(int index)
        {
            Control[] controls = Controls.Find($"button{index + 1}", true);
            return controls.Length > 0 ? controls[0] as Button : null;
        }

        // İndekse göre Etiket (Label) getirme
        private Label? GetLabelByIndex(int index)
        {
            Control[] controls = Controls.Find($"label{index + 3}", true);
            return controls.Length > 0 ? controls[0] as Label : null;
        }
    }
}