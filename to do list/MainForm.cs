using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace ToDoApp
{
    public partial class MainForm : Form
    {
        private DatabaseHelper dbHelper;
        private int userId;
        private ListView listViewTasks;

        public MainForm(int userId)
        {
            this.userId = userId;
            InitializeComponent();
            dbHelper = new DatabaseHelper();
            LoadTasks();
        }

        private void InitializeComponent()
        {
            this.Text = "to do list";
            this.Size = new System.Drawing.Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;

            
            TextBox txtTask = new TextBox { Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(400, 20), Name = "txtTask" };
            Button btnAdd = new Button { Text = "Add Task", Location = new System.Drawing.Point(430, 20), Size = new System.Drawing.Size(100, 30) };
            btnAdd.Click += BtnAdd_Click;

            
            listViewTasks = new ListView
            {
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(540, 250),
                View = View.Details,
                FullRowSelect = true
            };
            listViewTasks.Columns.Add("ID", 50);
            listViewTasks.Columns.Add("Description", 300);
            listViewTasks.Columns.Add("Completed", 100);
            listViewTasks.Columns.Add("Created Date", 120);

            
            Button btnEdit = new Button { Text = "Edit Task", Location = new System.Drawing.Point(20, 320), Size = new System.Drawing.Size(100, 30) };
            btnEdit.Click += BtnEdit_Click;

            Button btnDelete = new Button { Text = "Delete Task", Location = new System.Drawing.Point(130, 320), Size = new System.Drawing.Size(100, 30) };
            btnDelete.Click += BtnDelete_Click;

            Button btnToggleComplete = new Button { Text = "Toggle Complete", Location = new System.Drawing.Point(240, 320), Size = new System.Drawing.Size(120, 30) };
            btnToggleComplete.Click += BtnToggleComplete_Click;

            Button btnLogout = new Button { Text = "Logout", Location = new System.Drawing.Point(370, 320), Size = new System.Drawing.Size(100, 30) };
            btnLogout.Click += BtnLogout_Click;

            this.Controls.AddRange(new Control[] { txtTask, btnAdd, listViewTasks, btnEdit, btnDelete, btnToggleComplete, btnLogout });
        }

        private void LoadTasks()
        {
            listViewTasks.Items.Clear();
            using (SQLiteDataReader reader = dbHelper.GetTasks(userId))
            {
                while (reader.Read())
                {
                    ListViewItem item = new ListViewItem(reader["Id"].ToString());
                    item.SubItems.Add(reader["Description"].ToString());
                    item.SubItems.Add(Convert.ToBoolean(reader["IsCompleted"]) ? "Yes" : "No");
                    item.SubItems.Add(reader["CreatedDate"].ToString());
                    listViewTasks.Items.Add(item);
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string description = this.Controls["txtTask"].Text;
            if (!string.IsNullOrWhiteSpace(description))
            {
                dbHelper.AddTask(userId, description);
                this.Controls["txtTask"].Text = "";
                LoadTasks();
            }
            else
            {
                MessageBox.Show("Please enter a task description.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (listViewTasks.SelectedItems.Count > 0)
            {
                int taskId = int.Parse(listViewTasks.SelectedItems[0].SubItems[0].Text);
                string currentDescription = listViewTasks.SelectedItems[0].SubItems[1].Text;
                bool isCompleted = listViewTasks.SelectedItems[0].SubItems[2].Text == "Yes";

                string newDescription = PromptForTaskDescription(currentDescription);
                if (!string.IsNullOrEmpty(newDescription))
                {
                    dbHelper.UpdateTask(taskId, newDescription, isCompleted);
                    LoadTasks();
                }
            }
            else
            {
                MessageBox.Show("Please select a task to edit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (listViewTasks.SelectedItems.Count > 0)
            {
                int taskId = int.Parse(listViewTasks.SelectedItems[0].SubItems[0].Text);
                dbHelper.DeleteTask(taskId);
                LoadTasks();
            }
            else
            {
                MessageBox.Show("Please select a task to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnToggleComplete_Click(object sender, EventArgs e)
        {
            if (listViewTasks.SelectedItems.Count > 0)
            {
                int taskId = int.Parse(listViewTasks.SelectedItems[0].SubItems[0].Text);
                string description = listViewTasks.SelectedItems[0].SubItems[1].Text;
                bool isCompleted = listViewTasks.SelectedItems[0].SubItems[2].Text == "Yes";

                dbHelper.UpdateTask(taskId, description, !isCompleted);
                LoadTasks();
            }
            else
            {
                MessageBox.Show("Please select a task to toggle completion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private string PromptForTaskDescription(string currentDescription)
        {
            using (Form prompt = new Form())
            {
                prompt.Text = "Edit Task";
                prompt.Width = 300;
                prompt.Height = 150;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.StartPosition = FormStartPosition.CenterParent;

                Label label = new Label { Text = "Enter task description:", Left = 20, Top = 20 };
                TextBox textBox = new TextBox { Left = 20, Top = 40, Width = 200, Text = currentDescription };
                Button confirm = new Button { Text = "OK", Left = 20, Top = 70, Width = 80, DialogResult = DialogResult.OK };
                Button cancel = new Button { Text = "Cancel", Left = 110, Top = 70, Width = 80, DialogResult = DialogResult.Cancel };

                prompt.Controls.AddRange(new Control[] { label, textBox, confirm, cancel });
                prompt.AcceptButton = confirm;
                prompt.CancelButton = cancel;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
            }
        }
    }
}