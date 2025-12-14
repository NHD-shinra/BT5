using BT5.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BT5
{
    public partial class Form1 : Form
    {
        Model1 context = new Model1();
        public Form1()
        {
            InitializeComponent();
        }
        private void FillFalcultyCombobox(List<Faculty> listFalcultys)
        {
            this.cbb_ChuyenNganh.DataSource = listFalcultys;
            this.cbb_ChuyenNganh.DisplayMember = "FacultyName";
            this.cbb_ChuyenNganh.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            var listStudentWithFaculty = context.Student.Include("Faculty").ToList();

            foreach (var item in listStudentWithFaculty)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore;
            }
        }

        private void ResetForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtDiemTB.Clear();

            if (cbb_ChuyenNganh.Items.Count > 0)
            {
                cbb_ChuyenNganh.SelectedIndex = 0;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text) || string.IsNullOrWhiteSpace(txtHoTen.Text) || string.IsNullOrWhiteSpace(txtDiemTB.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtMaSV.Text.Length != 10)
            {
                MessageBox.Show("Mã số sinh viên phải có 10 kí tự!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!double.TryParse(txtDiemTB.Text, out double score))
            {
                MessageBox.Show("Điểm trung bình phải là số!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (context.Faculty.Count() == 0)
                {
                    context.Faculty.Add(new Faculty() { FacultyID = 1, FacultyName = "Công Nghệ Thông Tin" });
                    context.Faculty.Add(new Faculty() { FacultyID = 2, FacultyName = "Ngôn Ngữ Anh" });
                    context.Faculty.Add(new Faculty() { FacultyID = 3, FacultyName = "Quản Trị Kinh Doanh" });
                    context.SaveChanges();
                }

                List<Faculty> listFalcultys = context.Faculty.ToList();
                FillFalcultyCombobox(listFalcultys);

                List<Student> listStudent = context.Student.ToList();
                BindGrid(listStudent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối CSDL: {ex.Message}\nVui lòng kiểm tra lại Connection String và SQL Server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                Student existingStudent = context.Student.FirstOrDefault(s => s.StudentID == txtMaSV.Text);

                if (existingStudent != null)
                {
                    MessageBox.Show("Mã sinh viên này đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Student newStudent = new Student()
                {
                    StudentID = txtMaSV.Text,
                    FullName = txtHoTen.Text,
                    AverageScore = double.Parse(txtDiemTB.Text),
                    FacultyID = (int)cbb_ChuyenNganh.SelectedValue
                };

                context.Student.Add(newStudent);
                context.SaveChanges();

                BindGrid(context.Student.ToList());
                MessageBox.Show("Thêm mới dữ liệu thành công!", "Thông báo");
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                Student studentToUpdate = context.Student.FirstOrDefault(s => s.StudentID == txtMaSV.Text);

                if (studentToUpdate == null)
                {
                    MessageBox.Show("Không tìm thấy MSSV cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                studentToUpdate.FullName = txtHoTen.Text;
                studentToUpdate.AverageScore = double.Parse(txtDiemTB.Text);
                studentToUpdate.FacultyID = (int)cbb_ChuyenNganh.SelectedValue;

                context.SaveChanges();

                BindGrid(context.Student.ToList());
                MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo");
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng nhập MSSV cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Student studentToDelete = context.Student.FirstOrDefault(s => s.StudentID == txtMaSV.Text);

                if (studentToDelete == null)
                {
                    MessageBox.Show("Không tìm thấy MSSV cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên {studentToDelete.FullName} (MSSV: {txtMaSV.Text}) không?", "Cảnh báo Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    context.Student.Remove(studentToDelete);
                    context.SaveChanges();

                    BindGrid(context.Student.ToList());
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo");
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xóa dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvStudent.Rows.Count - 1)
            {
                DataGridViewRow selectedRow = dgvStudent.Rows[e.RowIndex];

                txtMaSV.Text = selectedRow.Cells[0].Value?.ToString();
                txtHoTen.Text = selectedRow.Cells[1].Value?.ToString();
                txtDiemTB.Text = selectedRow.Cells[3].Value?.ToString();

                // Hiển thị đúng khoa trong combobox
                string facultyName = selectedRow.Cells[2].Value?.ToString();
                if (facultyName != null)
                {
                    int index = cbb_ChuyenNganh.FindStringExact(facultyName);
                    if (index != -1)
                    {
                        cbb_ChuyenNganh.SelectedIndex = index;
                    }
                }
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}