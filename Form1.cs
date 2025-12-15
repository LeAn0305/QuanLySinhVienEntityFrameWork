using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLySinhVien_EntityFrameWork.Models;

namespace QuanLySinhVien_EntityFrameWork
{

    public partial class Form1 : Form
    {
        Model1 db = new Model1();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvStudent.ReadOnly = true;
            dgvStudent.AutoGenerateColumns = false;
            LoadFacultyToComBoBox();
            LoadStudentToDataGridView();
        }

        private void LoadFacultyToComBoBox()
        {
            var Faculties = db.Faculties.ToList();

            cbxKhoa.DataSource = Faculties;

            cbxKhoa.DisplayMember = "FacultyName";
            cbxKhoa.ValueMember = "FacultyID";
        }

        private void LoadStudentToDataGridView()
        {
            var Students = db.SinhViens.Select(s => new
            {
                MaSoSV = s.StudentID,
                TenSV = s.FullName,
                TenKhoa = s.Faculty.FacultyName,
                DiemTB = s.AverageScore,
            }).ToList();

            dgvStudent.DataSource = Students;

        }

        private void ClearInput()
        {
            txbMaSoSV.Clear();
            txbHoTen.Clear();
            txbDiemTB.Clear();
            cbxKhoa.SelectedIndex = 0;
            txbMaSoSV.Focus();
        }

        private bool CheckValidation(bool isAdding)
        {
            if(string.IsNullOrWhiteSpace(txbMaSoSV.Text) || string.IsNullOrWhiteSpace(txbHoTen.Text) || string.IsNullOrWhiteSpace(txbDiemTB.Text))
            {
                MessageBox.Show("Vui Lòng Nhập Đẩy Đủ Thông Tin Sinh Viên ! " , "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if(txbMaSoSV.Text.Length != 10 )
            {
                MessageBox.Show("Vui Lòng Nhập Mã Sinh Viên Có Đủ 10 Ký Tự !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if(!float.TryParse(txbDiemTB.Text , out float DiemTB))
            {
                MessageBox.Show("Nhập Sai Định Dạng Điểm ! ", " Lỗi",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if(DiemTB < 0 || DiemTB > 10)
            {
                MessageBox.Show("Vui Lòng Nhập Điểm TB Trong Khoảng ( 0 - 10 ) ", "lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if(isAdding) // kiểm tra trùng lặp mssv  
            {              
                var exitstingStudent = db.SinhViens.Find(txbMaSoSV.Text);
                if(exitstingStudent != null)
                {
                    MessageBox.Show("Mã Số Sinh Viên Này Đã Tồn Tại! ","Lỗi" , MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if(!CheckValidation(isAdding : true))
            {
                return;
            }

            try
            {
                int selectedFacultyID = (int)cbxKhoa.SelectedValue;
                float DiemTB = float.Parse(txbDiemTB.Text);

                SinhVien SV = new SinhVien
                {
                    StudentID = txbMaSoSV.Text,
                    FullName = txbHoTen.Text,
                    AverageScore = DiemTB,
                    FacultyID = selectedFacultyID
                };

                db.SinhViens.Add(SV);

                db.SaveChanges();   

                LoadStudentToDataGridView();
                ClearInput();

            }
            catch(Exception ex)
            {  
                MessageBox.Show("Lỗi Khi Thêm Sinh Viên :" +ex.Message , "Lỗi Database" ,MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if(!CheckValidation (isAdding : false))
            {
                return;
            }
            try
            {
                var MSSV  = db.SinhViens.Find(txbMaSoSV.Text.Trim());
                if(MSSV  == null)
                {
                    MessageBox.Show("Không Tìm Thấy Mã Số Sinh Viên ! ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MSSV.FullName = txbHoTen.Text.Trim();
                MSSV.AverageScore = float.Parse(txbDiemTB.Text);
                MSSV.FacultyID = (int)cbxKhoa.SelectedValue;
                db.SaveChanges();
                LoadStudentToDataGridView();
                ClearInput();
                MessageBox.Show("Cập Nhập Sinh Viên Thành Công ", "Thông Báo ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Khi Cập Nhập Sinh Viên ! " + ex.Message , "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string MSV = txbMaSoSV.Text.Trim();  

            if(string.IsNullOrWhiteSpace(MSV))
            {
                MessageBox.Show("Vui Lòng Nhập Mã Sinh Viên Cần Xóa ! ","Lỗi ",MessageBoxButtons.OK ,MessageBoxIcon.Error);
                return;
            }
            try
            {
                var SinhVienToDelete = db.SinhViens.Find(MSV);
                if(SinhVienToDelete == null)
                {
                    MessageBox.Show("Không Tìm Thấy Mã Sinh Viên Cần Xóa ! ", "Lỗi ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //string ComfirmMessage = $"Bạn Có Chắc Chắn Muốn Xóa Sinh Viên {SinhVienToDelete.FullName} , {MSV} Không ? ";
                DialogResult Result = MessageBox.Show(
                    $"Bạn Có Chắc Chắn Muốn Xóa Sinh Viên {SinhVienToDelete.FullName} , {MSV} Không ? "
                    , " Xác Nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if(Result == DialogResult.Yes)
                {
                    db.SinhViens.Remove(SinhVienToDelete);

                    db.SaveChanges();
                    LoadStudentToDataGridView();
                    ClearInput();
                    MessageBox.Show("Xóa Sinh Viên Thành Công ! ", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Lỗi Khi Xóa Sinh Viên " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dgvStudent_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStudent.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvStudent.SelectedRows[0];

                string maSV = row.Cells["MaSoSV"].Value?.ToString() ?? string.Empty;  // hẹ hẹ hơi khó hiểu chứ gì :)) , Comment mốt nhớ đọc lại nha an :)) Truy Cập Vào Cột Cells Có Tên MaSoSV lấy giá trị ( Value ) Nếu Nó Null  Thì Trả Về String Empty Nếu Không Null trả về chuỗi tosTRING
                string hoten = row.Cells["TenSV"].Value?.ToString() ?? string.Empty;
                string tenKhoa = row.Cells["TenKhoa"].Value?.ToString() ?? string.Empty;
                string diemTB = row.Cells["DiemTB"].Value?.ToString() ?? string.Empty;

                txbMaSoSV.Text = maSV;
                txbHoTen.Text = hoten;
                txbDiemTB.Text = diemTB;

                int index = cbxKhoa.FindStringExact(tenKhoa);
                if (index != -1)
                {
                    cbxKhoa.SelectedIndex = index;
                }
            }
            else
            {
                ClearInput();   
            }
        }
    }
}
