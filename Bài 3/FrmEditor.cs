using System.Text;
using System.Reflection;

namespace Bai3;

public sealed class FrmEditor : Form
{
    private readonly MenuStrip menuStrip1 = new();
    private readonly ToolStrip toolStrip1 = new();
    private readonly RichTextBox rtbNoiDung = new();
    private readonly StatusStrip statusStrip1 = new();
    private readonly ToolStripStatusLabel lblViTri = new();
    private readonly ToolStripStatusLabel lblKyTu = new();
    private readonly OpenFileDialog openFileDialog1 = new() { Filter = "Văn bản (*.txt)|*.txt|Tất cả (*.*)|*.*", CheckFileExists = true, RestoreDirectory = true };
    private readonly SaveFileDialog saveFileDialog1 = new() { Filter = "Văn bản (*.txt)|*.txt|Tất cả (*.*)|*.*", DefaultExt = "txt", AddExtension = true, RestoreDirectory = true };
    private readonly FontDialog fontDialog1 = new();
    private readonly ColorDialog colorDialog1 = new();
    private string duongDanFile = "";
    private bool daSua;
    private FrmTimThay? frmTimThay;

    public RichTextBox NoiDung => rtbNoiDung;

    public FrmEditor()
    {
        Name = nameof(FrmEditor);
        Text = "MiniEditor";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 900;
        Height = 650;
        MinimumSize = new Size(600, 400);
        TaoGiaoDien();
        GanTenControl();
        rtbNoiDung.TextChanged += rtbNoiDung_TextChanged;
        rtbNoiDung.SelectionChanged += rtbNoiDung_SelectionChanged;
        FormClosing += FrmEditor_FormClosing;
        CapNhatTrangThai();
    }

    private void TaoGiaoDien()
    {
        var mnuTep = new ToolStripMenuItem("Tệp") { Name = "mnuTep" };
        var mnuMoi = new ToolStripMenuItem("Mới", null, Moi_Click) { Name = "mnuMoi", ShortcutKeys = Keys.Control | Keys.N };
        var mnuMo = new ToolStripMenuItem("Mở", null, Mo_Click) { Name = "mnuMo", ShortcutKeys = Keys.Control | Keys.O };
        var mnuLuu = new ToolStripMenuItem("Lưu", null, Luu_Click) { Name = "mnuLuu", ShortcutKeys = Keys.Control | Keys.S };
        var mnuThoat = new ToolStripMenuItem("Thoát", null, (_, _) => Close()) { Name = "mnuThoat" };
        mnuTep.DropDownItems.AddRange(new ToolStripItem[] { mnuMoi, mnuMo, mnuLuu, new ToolStripSeparator(), mnuThoat });
        var mnuChinhSua = new ToolStripMenuItem("Chỉnh sửa") { Name = "mnuChinhSua" };
        var mnuCat = new ToolStripMenuItem("Cắt", null, (_, _) => rtbNoiDung.Cut()) { Name = "mnuCat", ShortcutKeys = Keys.Control | Keys.X };
        var mnuSaoChep = new ToolStripMenuItem("Sao chép", null, (_, _) => rtbNoiDung.Copy()) { Name = "mnuSaoChep", ShortcutKeys = Keys.Control | Keys.C };
        var mnuDan = new ToolStripMenuItem("Dán", null, (_, _) => rtbNoiDung.Paste()) { Name = "mnuDan", ShortcutKeys = Keys.Control | Keys.V };
        var mnuTimThay = new ToolStripMenuItem("Tìm và thay thế", null, (_, _) => MoTimThay()) { Name = "mnuTimThay", ShortcutKeys = Keys.Control | Keys.F };
        mnuChinhSua.DropDownItems.AddRange(new ToolStripItem[] { mnuCat, mnuSaoChep, mnuDan, new ToolStripSeparator(), mnuTimThay });
        var mnuDinhDang = new ToolStripMenuItem("Định dạng") { Name = "mnuDinhDang" };
        mnuDinhDang.DropDownItems.Add(new ToolStripMenuItem("Font...", null, (_, _) => ChonFont()) { Name = "mnuFont" });
        mnuDinhDang.DropDownItems.Add(new ToolStripMenuItem("Màu...", null, (_, _) => ChonMau()) { Name = "mnuMau" });
        menuStrip1.Items.AddRange(new ToolStripItem[] { mnuTep, mnuChinhSua, mnuDinhDang });
        menuStrip1.Dock = DockStyle.Top;

        toolStrip1.Items.Add(new ToolStripButton("Mới", null, Moi_Click) { Name = "tsbMoi" });
        toolStrip1.Items.Add(new ToolStripButton("Mở", null, Mo_Click) { Name = "tsbMo" });
        toolStrip1.Items.Add(new ToolStripButton("Lưu", null, Luu_Click) { Name = "tsbLuu" });
        toolStrip1.Items.Add(new ToolStripSeparator());
        toolStrip1.Items.Add(new ToolStripButton("Tìm", null, (_, _) => MoTimThay()) { Name = "tsbTimThay" });
        toolStrip1.Dock = DockStyle.Top;

        rtbNoiDung.Dock = DockStyle.Fill;
        rtbNoiDung.AcceptsTab = true;
        rtbNoiDung.Font = new Font("Segoe UI", 11F);
        lblViTri.Spring = true;
        statusStrip1.Items.Add(lblViTri);
        statusStrip1.Items.Add(lblKyTu);
        Controls.Add(rtbNoiDung);
        Controls.Add(statusStrip1);
        Controls.Add(toolStrip1);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
    }

    private void GanTenControl()
    {
        foreach (FieldInfo field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            if (field.GetValue(this) is Control control) control.Name = field.Name;
    }

    private void CapNhatTrangThai()
    {
        int viTri = rtbNoiDung.SelectionStart;
        int dong = rtbNoiDung.GetLineFromCharIndex(viTri);
        int dauDong = rtbNoiDung.GetFirstCharIndexFromLine(dong);
        int cot = viTri - Math.Max(0, dauDong);
        lblViTri.Text = $"Dòng {dong + 1}, Cột {cot + 1}";
        lblKyTu.Text = $"Số ký tự: {rtbNoiDung.TextLength}";
        string ten = string.IsNullOrEmpty(duongDanFile) ? "MiniEditor" : $"{Path.GetFileName(duongDanFile)} - MiniEditor";
        Text = $"{ten}{(daSua ? " *" : "")}";
    }

    private void rtbNoiDung_TextChanged(object? sender, EventArgs e) { daSua = true; CapNhatTrangThai(); }
    private void rtbNoiDung_SelectionChanged(object? sender, EventArgs e) => CapNhatTrangThai();

    private bool HoiLuu()
    {
        if (!daSua) return true;
        var ketQua = MessageBox.Show("Nội dung đã thay đổi. Lưu trước khi tiếp tục?", "Mini Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        if (ketQua == DialogResult.Cancel) return false;
        return ketQua != DialogResult.Yes || Luu();
    }

    private bool Luu()
    {
        if (string.IsNullOrEmpty(duongDanFile))
        {
            saveFileDialog1.InitialDirectory = string.IsNullOrWhiteSpace(saveFileDialog1.InitialDirectory)
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                : saveFileDialog1.InitialDirectory;
            saveFileDialog1.FileName = "van-ban-moi.txt";
            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK) return false;
            duongDanFile = saveFileDialog1.FileName;
        }
        try { File.WriteAllText(duongDanFile, rtbNoiDung.Text, new UTF8Encoding(true)); }
        catch (IOException ex) { MessageBox.Show($"Không lưu được: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
        daSua = false;
        CapNhatTrangThai();
        return true;
    }

    private void Moi_Click(object? sender, EventArgs e)
    {
        if (!HoiLuu()) return;
        rtbNoiDung.Clear();
        duongDanFile = "";
        daSua = false;
        CapNhatTrangThai();
    }

    private void Mo_Click(object? sender, EventArgs e)
    {
        if (!HoiLuu() || openFileDialog1.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            rtbNoiDung.Text = File.ReadAllText(openFileDialog1.FileName, Encoding.UTF8);
            duongDanFile = openFileDialog1.FileName;
            daSua = false;
            CapNhatTrangThai();
        }
        catch (IOException ex) { MessageBox.Show($"Không mở được: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void Luu_Click(object? sender, EventArgs e) => Luu();

    private void ChonFont()
    {
        fontDialog1.Font = rtbNoiDung.Font;
        if (fontDialog1.ShowDialog(this) == DialogResult.OK) rtbNoiDung.Font = fontDialog1.Font;
    }

    private void ChonMau()
    {
        colorDialog1.Color = rtbNoiDung.ForeColor;
        if (colorDialog1.ShowDialog(this) == DialogResult.OK) rtbNoiDung.ForeColor = colorDialog1.Color;
    }

    private void MoTimThay()
    {
        if (frmTimThay is null || frmTimThay.IsDisposed) frmTimThay = new FrmTimThay(rtbNoiDung);
        frmTimThay.Show(this);
        frmTimThay.BringToFront();
    }

    private void FrmEditor_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!HoiLuu()) e.Cancel = true;
    }
}
