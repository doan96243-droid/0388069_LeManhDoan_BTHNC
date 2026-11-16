using System.Globalization;
using System.Reflection;
using System.Text;

namespace Bai1;

public sealed class FrmSinhVien : Form
{
    private readonly List<SinhVien> goc = new();
    private readonly BindingSource bs = new();
    private readonly DataGridView dgv = new();
    private readonly TextBox txtTimKiem = new();
    private readonly TextBox txtMa = new();
    private readonly TextBox txtHoTen = new();
    private readonly ComboBox cboLop = new();
    private readonly NumericUpDown nudDiem = new();
    private readonly Label lblThongKe = new();
    private readonly Button btnThem = new();
    private readonly Button btnSua = new();
    private readonly Button btnXoa = new();
    private readonly Button btnXuatCsv = new();
    private readonly SaveFileDialog saveFileDialog1 = new() { Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*", FileName = "DanhSachSinhVien.csv", RestoreDirectory = true };

    public FrmSinhVien()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        Name = nameof(FrmSinhVien);
        Text = "Quản lý sinh viên";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(850, 500);
        Width = 1050;
        Height = 650;
        TaoGiaoDien();
        GanTenControl();
        Load += FrmSinhVien_Load;
    }

    private void TaoGiaoDien()
    {
        var thongTin = new TableLayoutPanel { Name = "pnlThongTin", Dock = DockStyle.Top, Height = 105, Padding = new Padding(8), ColumnCount = 6, RowCount = 2 };
        thongTin.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        thongTin.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        thongTin.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        thongTin.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        thongTin.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        thongTin.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        thongTin.Controls.Add(new Label { Text = "Tìm kiếm:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        txtTimKiem.PlaceholderText = "Tìm theo mã hoặc tên";
        txtTimKiem.Dock = DockStyle.Fill;
        thongTin.Controls.Add(txtTimKiem, 1, 0);
        thongTin.Controls.Add(new Label { Text = "Mã SV:", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 0);
        txtMa.Dock = DockStyle.Fill;
        thongTin.Controls.Add(txtMa, 3, 0);
        thongTin.Controls.Add(new Label { Text = "Họ tên:", AutoSize = true, Anchor = AnchorStyles.Left }, 4, 0);
        txtHoTen.Dock = DockStyle.Fill;
        thongTin.Controls.Add(txtHoTen, 5, 0);
        thongTin.Controls.Add(new Label { Text = "Lớp:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        cboLop.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLop.Items.AddRange(new object[] { "64CNTT1", "64CNTT2", "64PM1" });
        cboLop.Dock = DockStyle.Fill;
        thongTin.Controls.Add(cboLop, 1, 1);
        thongTin.Controls.Add(new Label { Text = "Điểm:", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 1);
        nudDiem.Minimum = 0; nudDiem.Maximum = 10; nudDiem.DecimalPlaces = 1; nudDiem.Increment = 0.5m; nudDiem.Dock = DockStyle.Fill;
        thongTin.Controls.Add(nudDiem, 3, 1);
        btnThem.Text = "Thêm"; btnThem.Click += btnThem_Click; thongTin.Controls.Add(btnThem, 4, 1);
        btnSua.Text = "Sửa"; btnSua.Click += btnSua_Click; thongTin.Controls.Add(btnSua, 5, 1);

        var thanhDuoi = new FlowLayoutPanel { Name = "pnlThanhDuoi", Dock = DockStyle.Bottom, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, MinimumSize = new Size(0, 54), Padding = new Padding(8), WrapContents = false };
        btnThem.AutoSize = true; btnSua.AutoSize = true;
        btnXoa.Text = "Xóa"; btnXoa.AutoSize = true; btnXoa.Click += btnXoa_Click;
        btnXuatCsv.Text = "Xuất CSV"; btnXuatCsv.AutoSize = true; btnXuatCsv.Click += btnXuatCsv_Click;
        lblThongKe.AutoSize = true; lblThongKe.Padding = new Padding(12, 8, 0, 0);
        thanhDuoi.Controls.AddRange(new Control[] { btnXoa, btnXuatCsv, lblThongKe });

        dgv.Dock = DockStyle.Fill;
        dgv.ReadOnly = true; dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.AllowUserToAddRows = false;
        dgv.AutoGenerateColumns = true; dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.CellFormatting += dgv_CellFormatting;
        txtTimKiem.TextChanged += txtTimKiem_TextChanged;
        Controls.Add(dgv); Controls.Add(thanhDuoi); Controls.Add(thongTin);
    }

    private void GanTenControl()
    {
        foreach (FieldInfo field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            if (field.GetValue(this) is Control control) control.Name = field.Name;
    }

    private void FrmSinhVien_Load(object? sender, EventArgs e)
    {
        goc.AddRange(new[] {
            new SinhVien("SV01", "Nguyen Van An", "64CNTT1", 8.5), new SinhVien("SV02", "Tran Thi Binh", "64CNTT1", 7.0),
            new SinhVien("SV03", "Le Van Cuong", "64CNTT2", 5.5), new SinhVien("SV04", "Pham Thu Dung", "64CNTT2", 9.0),
            new SinhVien("SV05", "Hoang Minh Duc", "64PM1", 4.5), new SinhVien("SV06", "Vu Thanh Ha", "64PM1", 6.5),
            new SinhVien("SV07", "Do Quang Huy", "64PM1", 8.0), new SinhVien("SV08", "Bui Lan Huong", "64CNTT1", 7.5)
        });
        cboLop.SelectedIndex = 0;
        dgv.DataSource = bs;
        if (dgv.Columns[nameof(SinhVien.Diem)] is DataGridViewColumn cotDiem)
        {
            cotDiem.DefaultCellStyle.Format = "0.0";
            cotDiem.DefaultCellStyle.FormatProvider = CultureInfo.InvariantCulture;
        }
        bs.CurrentChanged += bs_CurrentChanged;
        LamMoi();
    }

    private void LamMoi(string? maChon = null)
    {
        string kw = txtTimKiem.Text.Trim();
        var kq = goc.Where(s => s.MaSV.Contains(kw, StringComparison.OrdinalIgnoreCase) || s.HoTen.Contains(kw, StringComparison.OrdinalIgnoreCase)).ToList();
        bs.DataSource = kq;
        lblThongKe.Text = $"Hiển thị {kq.Count}/{goc.Count} sinh viên" + (kq.Count > 0 ? $" | Điểm TB: {kq.Average(s => s.Diem).ToString("F2", CultureInfo.InvariantCulture)}" : "");
        if (maChon != null) { int vt = kq.FindIndex(s => s.MaSV == maChon); if (vt >= 0) bs.Position = vt; }
    }

    private void txtTimKiem_TextChanged(object? sender, EventArgs e) => LamMoi();

    private void bs_CurrentChanged(object? sender, EventArgs e)
    {
        if (bs.Current is SinhVien sv) { txtMa.Text = sv.MaSV; txtHoTen.Text = sv.HoTen; cboLop.SelectedItem = sv.Lop; nudDiem.Value = (decimal)sv.Diem; txtMa.ReadOnly = true; }
        else txtMa.ReadOnly = false;
    }

    private void dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgv.Rows[e.RowIndex];
        if (row.DataBoundItem is SinhVien sv && sv.Diem < 5 && e.CellStyle is DataGridViewCellStyle style) { style.BackColor = Color.MistyRose; style.ForeColor = Color.DarkRed; }
    }

    private void btnThem_Click(object? sender, EventArgs e)
    {
        string ma = txtMa.Text.Trim(), ten = txtHoTen.Text.Trim();
        if (ma == "" || ten == "" || cboLop.SelectedIndex < 0) { MessageBox.Show("Nhập đầy đủ mã, họ tên và chọn lớp."); return; }
        if (goc.Any(s => s.MaSV.Equals(ma, StringComparison.OrdinalIgnoreCase))) { MessageBox.Show("Mã sinh viên đã tồn tại."); return; }
        double diem = (double)nudDiem.Value;
        goc.Add(new SinhVien(ma, ten, cboLop.Text, diem)); txtTimKiem.Clear(); LamMoi(ma);
        if (bs.Current is SinhVien sv && sv.MaSV == ma) nudDiem.Value = (decimal)diem;
    }

    private void btnSua_Click(object? sender, EventArgs e)
    {
        if (bs.Current is not SinhVien sv) return;
        if (string.IsNullOrWhiteSpace(txtHoTen.Text) || cboLop.SelectedIndex < 0) { MessageBox.Show("Họ tên và lớp không được bỏ trống."); return; }
        sv.HoTen = txtHoTen.Text.Trim(); sv.Lop = cboLop.Text; sv.Diem = (double)nudDiem.Value; LamMoi(sv.MaSV);
    }

    private void btnXoa_Click(object? sender, EventArgs e)
    {
        if (bs.Current is not SinhVien sv) return;
        if (MessageBox.Show($"Xóa sinh viên {sv.MaSV}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) { goc.Remove(sv); LamMoi(); }
    }

    private void btnXuatCsv_Click(object? sender, EventArgs e)
    {
        if (saveFileDialog1.ShowDialog(this) != DialogResult.OK) return;
        string Escape(string value) => $"\"{value.Replace("\"", "\"\"")}\"";
        string kw = txtTimKiem.Text.Trim();
        var hienThi = goc.Where(s => s.MaSV.Contains(kw, StringComparison.OrdinalIgnoreCase) || s.HoTen.Contains(kw, StringComparison.OrdinalIgnoreCase));
        string fileName = saveFileDialog1.FileName;
        using var writer = new StreamWriter(fileName, false, new UTF8Encoding(true));
        writer.WriteLine("MaSV,HoTen,Lop,Diem");
        foreach (var sv in hienThi) writer.WriteLine($"{Escape(sv.MaSV)},{Escape(sv.HoTen)},{Escape(sv.Lop)},{sv.Diem.ToString(CultureInfo.InvariantCulture)}");
    }
}
