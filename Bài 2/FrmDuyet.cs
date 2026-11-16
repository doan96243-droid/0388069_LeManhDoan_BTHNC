using System.Reflection;
using System.Security;

namespace Bai2;

public sealed class FrmDuyet : Form
{
    private readonly SplitContainer splitContainer1 = new();
    private readonly TreeView tvThuMuc = new();
    private readonly ListView lvTapTin = new();
    private readonly StatusStrip statusStrip1 = new();
    private readonly ToolStripStatusLabel lblTrangThai = new();
    private readonly ImageList imageList1 = new();

    public FrmDuyet()
    {
        Name = nameof(FrmDuyet);
        Text = "Duyệt thư mục";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 1200;
        Height = 700;
        MinimumSize = new Size(700, 400);
        TaoGiaoDien();
        GanTenControl();
        Load += FrmDuyet_Load;
        Shown += (_, _) => DatTyLeKhung();
        Resize += (_, _) => DatTyLeKhung();
    }

    private void GanTenControl()
    {
        foreach (FieldInfo field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            if (field.GetValue(this) is Control control) control.Name = field.Name;
    }

    private void TaoGiaoDien()
    {
        imageList1.ImageSize = new Size(16, 16);
        imageList1.Images.Add(TaoBieuTuongThuMuc());
        imageList1.Images.Add(TaoBieuTuongTapTin());

        splitContainer1.Dock = DockStyle.Fill;
        splitContainer1.Panel1.Controls.Add(tvThuMuc);
        splitContainer1.Panel2.Controls.Add(lvTapTin);
        tvThuMuc.Dock = DockStyle.Fill;
        tvThuMuc.ImageList = imageList1;
        lvTapTin.Dock = DockStyle.Fill;
        lvTapTin.View = View.Details;
        lvTapTin.FullRowSelect = true;
        lvTapTin.GridLines = true;
        lvTapTin.MultiSelect = false;
        lvTapTin.SmallImageList = imageList1;
        lvTapTin.Columns.Add("Tên", 280);
        lvTapTin.Columns.Add("Loại", 120);
        lvTapTin.Columns.Add("Kích thước (KB)", 120, HorizontalAlignment.Right);
        lvTapTin.Columns.Add("Ngày sửa", 160);
        lvTapTin.Resize += (_, _) => DatDoRongCot();
        tvThuMuc.BeforeExpand += tvThuMuc_BeforeExpand;
        tvThuMuc.AfterSelect += tvThuMuc_AfterSelect;
        lvTapTin.DoubleClick += lvTapTin_DoubleClick;
        statusStrip1.Items.Add(lblTrangThai);
        statusStrip1.Dock = DockStyle.Bottom;
        Controls.Add(splitContainer1);
        Controls.Add(statusStrip1);
    }

    private void DatTyLeKhung()
    {
        int width = splitContainer1.ClientSize.Width;
        if (width < 500) return;
        splitContainer1.Panel1MinSize = 260;
        splitContainer1.Panel2MinSize = Math.Min(560, width - 260 - splitContainer1.SplitterWidth - 20);
        int maxDistance = width - splitContainer1.Panel2MinSize - splitContainer1.SplitterWidth;
        splitContainer1.SplitterDistance = Math.Clamp((int)(width * 0.33), splitContainer1.Panel1MinSize, maxDistance);
        DatDoRongCot();
    }

    private void DatDoRongCot()
    {
        if (lvTapTin.Columns.Count != 4) return;
        int width = Math.Max(0, lvTapTin.ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
        lvTapTin.Columns[0].Width = (int)(width * 0.34);
        lvTapTin.Columns[1].Width = (int)(width * 0.17);
        lvTapTin.Columns[2].Width = (int)(width * 0.23);
        lvTapTin.Columns[3].Width = Math.Max(0, width - lvTapTin.Columns.Cast<ColumnHeader>().Take(3).Sum(column => column.Width));
    }

    private static Bitmap TaoBieuTuongThuMuc()
    {
        var bitmap = new Bitmap(16, 16);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);
        using var brush = new SolidBrush(Color.Goldenrod);
        using var light = new SolidBrush(Color.Khaki);
        graphics.FillRectangle(brush, 1, 5, 14, 9);
        graphics.FillRectangle(light, 2, 3, 6, 4);
        graphics.DrawRectangle(Pens.SaddleBrown, 1, 5, 13, 8);
        return bitmap;
    }

    private static Bitmap TaoBieuTuongTapTin()
    {
        var bitmap = new Bitmap(16, 16);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);
        using var brush = new SolidBrush(Color.WhiteSmoke);
        using var pen = new Pen(Color.SlateGray);
        Point[] page = { new(3, 1), new(10, 1), new(13, 4), new(13, 15), new(3, 15) };
        graphics.FillPolygon(brush, page);
        graphics.DrawPolygon(pen, page);
        graphics.DrawLine(pen, 5, 7, 11, 7);
        graphics.DrawLine(pen, 5, 10, 11, 10);
        graphics.DrawLine(pen, 5, 13, 10, 13);
        return bitmap;
    }

    private void FrmDuyet_Load(object? sender, EventArgs e)
    {
        foreach (DriveInfo drive in DriveInfo.GetDrives().Where(d => d.IsReady))
            ThemNut(tvThuMuc.Nodes, drive.Name, drive.RootDirectory.FullName);
        lblTrangThai.Text = $"Số mục: {tvThuMuc.Nodes.Count}";
    }

    private void ThemNut(TreeNodeCollection cha, string ten, string duongDan)
    {
        var nut = new TreeNode(ten) { Tag = duongDan, ImageIndex = 0, SelectedImageIndex = 0 };
        nut.Nodes.Add(new TreeNode("...") { Tag = null });
        cha.Add(nut);
    }

    private void tvThuMuc_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
    {
        TreeNode? nut = e.Node;
        if (nut is null) return;
        if (nut.Nodes.Count != 1 || nut.Nodes[0].Text != "...") return;
        nut.Nodes.Clear();
        try
        {
            foreach (string path in Directory.GetDirectories((string)nut.Tag!))
                ThemNut(nut.Nodes, Path.GetFileName(path), path);
        }
        catch (UnauthorizedAccessException) { lblTrangThai.Text = "Không có quyền truy cập thư mục."; }
        catch (SecurityException) { lblTrangThai.Text = "Không có quyền truy cập thư mục."; }
        catch (IOException) { lblTrangThai.Text = "Không thể đọc thư mục hoặc ổ đĩa."; }
    }

    private void tvThuMuc_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node is TreeNode node && node.Tag is string path) NapDanhSach(path);
    }

    private void NapDanhSach(string duongDan)
    {
        lvTapTin.BeginUpdate();
        lvTapTin.Items.Clear();
        try
        {
            var thuMuc = new DirectoryInfo(duongDan);
            foreach (DirectoryInfo directory in thuMuc.GetDirectories())
            {
                var item = new ListViewItem(directory.Name, 0);
                item.SubItems.Add("Thư mục"); item.SubItems.Add(""); item.SubItems.Add(directory.LastWriteTime.ToString("dd/MM/yyyy HH:mm"));
                item.Tag = directory.FullName; lvTapTin.Items.Add(item);
            }
            foreach (FileInfo file in thuMuc.GetFiles())
            {
                var item = new ListViewItem(file.Name, 1);
                item.SubItems.Add(file.Extension.Length == 0 ? "Tệp" : file.Extension); item.SubItems.Add((file.Length / 1024d).ToString("F1")); item.SubItems.Add(file.LastWriteTime.ToString("dd/MM/yyyy HH:mm"));
                item.Tag = file.FullName; lvTapTin.Items.Add(item);
            }
            lblTrangThai.Text = $"{duongDan} | Số mục: {lvTapTin.Items.Count}";
        }
        catch (UnauthorizedAccessException) { lblTrangThai.Text = $"{duongDan} | Không có quyền truy cập"; }
        catch (SecurityException) { lblTrangThai.Text = $"{duongDan} | Không có quyền truy cập"; }
        catch (IOException) { lblTrangThai.Text = $"{duongDan} | Không thể đọc thư mục"; }
        finally { lvTapTin.EndUpdate(); }
    }

    private void lvTapTin_DoubleClick(object? sender, EventArgs e)
    {
        if (lvTapTin.SelectedItems.Count == 0 || lvTapTin.SelectedItems[0].Tag is not string path || !Directory.Exists(path)) return;
        TreeNode? node = TimNut(tvThuMuc.Nodes, path);
        if (node == null) return;
        node.Expand();
        tvThuMuc.SelectedNode = node;
        node.EnsureVisible();
    }

    private static TreeNode? TimNut(TreeNodeCollection nodes, string path)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Tag is string current && string.Equals(current, path, StringComparison.OrdinalIgnoreCase)) return node;
            TreeNode? found = TimNut(node.Nodes, path);
            if (found != null) return found;
        }
        return null;
    }
}
