using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ToDoApp
{
    public partial class LoginForm : Form
    {
        private DatabaseHelper dbHelper;

        public LoginForm()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
        }

        private void InitializeComponent()
        {
            this.Text = "Login";
            this.Size = new System.Drawing.Size(350, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = Color.White;

            
            Panel panel = new Panel
            {
                Size = new System.Drawing.Size(350, 250),
                Location = new System.Drawing.Point(0, 0),
                BackColor = Color.Transparent
            };
            panel.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(panel.ClientRectangle, Color.LightSkyBlue, Color.WhiteSmoke, LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, panel.ClientRectangle);
                }
            };

            
            Label lblUsername = new Label
            {
                Text = "Username:",
                Location = new System.Drawing.Point(50, 40),
                ForeColor = Color.DarkSlateGray,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            TextBox txtUsername = new TextBox
            {
                Name = "txtUsername",
                Location = new System.Drawing.Point(50, 65),
                Size = new System.Drawing.Size(250, 25),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            Label lblPassword = new Label
            {
                Text = "Password:",
                Location = new System.Drawing.Point(50, 100),
                ForeColor = Color.DarkSlateGray,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            TextBox txtPassword = new TextBox
            {
                Name = "txtPassword",
                Location = new System.Drawing.Point(50, 125),
                Size = new System.Drawing.Size(250, 25),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black,
                UseSystemPasswordChar = true
            };

            
            Button btnLogin = new Button
            {
                Text = "Login",
                Location = new System.Drawing.Point(100, 170),
                Size = new System.Drawing.Size(80, 30),
                BackColor = Color.LightSteelBlue,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnLogin.Click += BtnLogin_Click;

            Button btnRegister = new Button
            {
                Text = "Register",
                Location = new System.Drawing.Point(190, 170),
                Size = new System.Drawing.Size(80, 30),
                BackColor = Color.LightSteelBlue,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnRegister.Click += BtnRegister_Click;

            
            panel.Controls.AddRange(new Control[] { lblUsername, txtUsername, lblPassword, txtPassword, btnLogin, btnRegister });
            this.Controls.Add(panel);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = this.Controls[0].Controls["txtUsername"].Text;
            string password = this.Controls[0].Controls["txtPassword"].Text;

            int? userId = dbHelper.AuthenticateUser(username, password);
            if (userId.HasValue)
            {
                MainForm mainForm = new MainForm(userId.Value);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.ShowDialog();
        }
    }
}