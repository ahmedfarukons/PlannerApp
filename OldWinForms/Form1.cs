using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace StudyPlanner
{
    public partial class Form1 : Form
    {
        private BindingSource bindingSource;
        private List<StudyPlanItem> items;
        private Panel selectedCard;

        public Form1()
        {
            InitializeComponent();
            InitializeDataModel();

            this.Font = new Font("Segoe UI", 9F);
            this.BackColor = Color.FromArgb(250, 250, 250);
            panelTop.BackColor = Color.White;
            foreach (var b in new[] { btnAdd, btnDelete, btnSave, btnLoad })
            {
                b.FlatStyle = FlatStyle.Flat;
                b.BackColor = Color.FromArgb(243, 244, 246);
                b.FlatAppearance.BorderColor = Color.FromArgb(229, 231, 235);
            }

            listPanel.SizeChanged += (s, e) => AdjustCardWidths();
            RenderList();
        }

        private void InitializeDataModel()
        {
            items = new List<StudyPlanItem>();
            bindingSource = new BindingSource();
            bindingSource.DataSource = items;
        }

        private void RenderList()
        {
            listPanel.SuspendLayout();
            listPanel.Controls.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                var card = CreateCard(items[i], i);
                listPanel.Controls.Add(card);
            }
            listPanel.ResumeLayout();
            AdjustCardWidths();
        }

        private void AdjustCardWidths()
        {
            int width = listPanel.ClientSize.Width - listPanel.Padding.Left - listPanel.Padding.Right - 2;
            foreach (Control c in listPanel.Controls)
            {
                c.Width = Math.Max(300, width);
            }
        }

        private Panel CreateCard(StudyPlanItem item, int index)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Margin = new Padding(8),
                Padding = new Padding(12),
                Height = 88,
                Tag = index
            };

            var lblTitle = new Label
            {
                AutoSize = false,
                Text = item.Subject,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 36, 36),
                Dock = DockStyle.Top,
                Height = 24
            };

            var lblMeta = new Label
            {
                AutoSize = false,
                Text = string.Format("{0:dd.MM.yyyy HH:mm} • {1} dk", item.Date, item.DurationMinutes),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(107, 114, 128),
                Dock = DockStyle.Top,
                Height = 20
            };

            var lblNotes = new Label
            {
                AutoSize = false,
                Text = string.IsNullOrWhiteSpace(item.Notes) ? " " : item.Notes,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(55, 65, 81),
                Dock = DockStyle.Fill
            };

            card.Controls.Add(lblNotes);
            card.Controls.Add(lblMeta);
            card.Controls.Add(lblTitle);

            card.Resize += (s, e) => SetRoundedRegion(card, 8);
            card.Cursor = Cursors.Hand;
            card.MouseEnter += (s, e) => { if (card != selectedCard) card.BackColor = Color.FromArgb(250, 250, 250); };
            card.MouseLeave += (s, e) => { if (card != selectedCard) card.BackColor = Color.White; };
            card.Click += (s, e) => SelectCard(card);

            return card;
        }

        private void SelectCard(Panel card)
        {
            if (selectedCard != null && selectedCard != card)
                selectedCard.BackColor = Color.White;

            selectedCard = card;
            selectedCard.BackColor = Color.FromArgb(236, 245, 255);
        }

        private void SetRoundedRegion(Control c, int radius)
        {
            var rect = c.ClientRectangle;
            if (rect.Width == 0 || rect.Height == 0) return;

            using (var path = new GraphicsPath())
            {
                int d = radius * 2;
                path.StartFigure();
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                c.Region = new Region(path);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSubject.Text))
            {
                MessageBox.Show("Ders/Konu boş olamaz.");
                return;
            }

            var newItem = new StudyPlanItem
            {
                Date = dtpDate.Value,
                DurationMinutes = (int)numDuration.Value,
                Subject = txtSubject.Text.Trim(),
                Notes = txtNotes.Text.Trim()
            };
            items.Add(newItem);
            RenderList();

            txtSubject.Clear();
            txtNotes.Clear();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCard == null) return;
            int idx = (int)selectedCard.Tag;
            if (idx < 0 || idx >= items.Count) return;

            items.RemoveAt(idx);
            selectedCard = null;
            RenderList();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "XML Files (*.xml)|*.xml";
                sfd.FileName = "plans.xml";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var serializer = new XmlSerializer(typeof(List<StudyPlanItem>));
                        using (var fs = File.Open(sfd.FileName, FileMode.Create))
                        {
                            serializer.Serialize(fs, items);
                        }
                        MessageBox.Show("Kaydedildi.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Kaydetme hatası: " + ex.Message);
                    }
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "XML Files (*.xml)|*.xml";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var serializer = new XmlSerializer(typeof(List<StudyPlanItem>));
                        using (var fs = File.OpenRead(ofd.FileName))
                        {
                            var loaded = (List<StudyPlanItem>)serializer.Deserialize(fs);
                            items = loaded ?? new List<StudyPlanItem>();
                            RenderList();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Yükleme hatası: " + ex.Message);
                    }
                }
            }
        }
    }

    public class StudyPlanItem
    {
        public DateTime Date { get; set; }
        public int DurationMinutes { get; set; }
        public string Subject { get; set; }
        public string Notes { get; set; }
    }
}