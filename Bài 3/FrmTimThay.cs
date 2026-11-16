namespace Bai3;

public sealed class FrmTimThay : Form
{
    private readonly RichTextBox rtbNoiDung;
    private readonly TextBox txtTim = new();
    private readonly TextBox txtThay = new();
    private readonly CheckBox chkPhanBietHoaThuong = new() { Text = "Phân biệt hoa thường", AutoSize = true };
    private readonly Label lblKetQua = new() { AutoSize = true };

    public FrmTimThay(RichTextBox richTextBox)
    {
        rtbNoiDung = richTextBox;
        Name = nameof(FrmTimThay);
        txtTim.Name = nameof(txtTim);
        txtThay.Name = nameof(txtThay);
        chkPhanBietHoaThuong.Name = nameof(chkPhanBietHoaThuong);
        lblKetQua.Name = nameof(lblKetQua);
        Text = "Tìm và thay thế";
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        Width = 430;
        Height = 190;
        TaoGiaoDien();
    }

    private void TaoGiaoDien()
    {
        var bang = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 2, RowCount = 4 };
        bang.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bang.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        bang.Controls.Add(new Label { Text = "Tìm:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        txtTim.Dock = DockStyle.Fill;
        bang.Controls.Add(txtTim, 1, 0);
        bang.Controls.Add(new Label { Text = "Thay bằng:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        txtThay.Dock = DockStyle.Fill;
        bang.Controls.Add(txtThay, 1, 1);
        bang.Controls.Add(chkPhanBietHoaThuong, 1, 2);
        var nut = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        var btnTim = new Button { Name = "btnTimTiep", Text = "Tìm tiếp", AutoSize = true };
        var btnThayTatCa = new Button { Name = "btnThayTatCa", Text = "Thay tất cả", AutoSize = true };
        btnTim.Click += (_, _) => TimTiep();
        btnThayTatCa.Click += (_, _) => ThayTatCa();
        nut.Controls.Add(btnTim); nut.Controls.Add(btnThayTatCa); nut.Controls.Add(lblKetQua);
        bang.Controls.Add(nut, 1, 3);
        Controls.Add(bang);
        AcceptButton = btnTim;
    }

    private RichTextBoxFinds TimKiemOptions()
    {
        return chkPhanBietHoaThuong.Checked ? RichTextBoxFinds.MatchCase : RichTextBoxFinds.None;
    }

    private void TimTiep()
    {
        if (string.IsNullOrEmpty(txtTim.Text)) { lblKetQua.Text = "Nhập nội dung cần tìm."; return; }
        int batDau = rtbNoiDung.SelectionStart + rtbNoiDung.SelectionLength;
        int viTri = rtbNoiDung.Find(txtTim.Text, batDau, TimKiemOptions());
        if (viTri < 0 && batDau > 0) viTri = rtbNoiDung.Find(txtTim.Text, 0, TimKiemOptions());
        if (viTri >= 0) { rtbNoiDung.Select(viTri, txtTim.Text.Length); rtbNoiDung.ScrollToCaret(); lblKetQua.Text = "Đã tìm thấy."; }
        else lblKetQua.Text = "Không tìm thấy.";
    }

    private void ThayTatCa()
    {
        if (string.IsNullOrEmpty(txtTim.Text)) { lblKetQua.Text = "Nhập nội dung cần tìm."; return; }
        int soLan = 0;
        int viTri = 0;
        RichTextBoxFinds options = TimKiemOptions();
        while (viTri < rtbNoiDung.TextLength)
        {
            int timThay = rtbNoiDung.Find(txtTim.Text, viTri, options);
            if (timThay < 0) break;
            rtbNoiDung.Select(timThay, txtTim.Text.Length);
            rtbNoiDung.SelectedText = txtThay.Text;
            viTri = timThay + txtThay.Text.Length;
            soLan++;
        }
        lblKetQua.Text = $"Đã thay {soLan} lần.";
    }
}
