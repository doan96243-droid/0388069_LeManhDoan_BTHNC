using Bai1;
using Bai2;
using Bai3;
using System.Reflection;

namespace Bai4;

public sealed class FrmMain : Form
{
    private readonly MenuStrip menuStrip1 = new();
    private readonly ToolStripMenuItem mnuCuaSo = new("Cửa sổ") { Name = "mnuCuaSo" };
    private readonly StatusStrip statusStrip1 = new();
    private readonly ToolStripStatusLabel lblGio = new();
    private readonly ToolStripStatusLabel lblSoCuaSo = new();
    private readonly System.Windows.Forms.Timer tmrDongHo = new() { Interval = 1000, Enabled = true };

    public FrmMain()
    {
        Name = nameof(FrmMain);
        Text = "Quản lý - MDI";
        IsMdiContainer = true;
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(1200, 800);
        MainMenuStrip = menuStrip1;
        TaoGiaoDien();
        GanTenControl();
        Load += FrmMain_Load;
        FormClosing += FrmMain_FormClosing;
        MdiChildActivate += (_, _) => CapNhatSoCuaSo();
        tmrDongHo.Tick += tmrDongHo_Tick;
    }

    private void TaoGiaoDien()
    {
        var mnuQuanLy = new ToolStripMenuItem("Quản lý") { Name = "mnuQuanLy" };
        mnuQuanLy.DropDownItems.Add(new ToolStripMenuItem("Sinh viên", null, (_, _) => MoFormCon<FrmSinhVien>()) { Name = "mnuSinhVien" });
        mnuQuanLy.DropDownItems.Add(new ToolStripMenuItem("Mini Editor", null, (_, _) => MoFormCon<FrmEditor>()) { Name = "mnuEditor" });
        mnuQuanLy.DropDownItems.Add(new ToolStripMenuItem("Duyệt thư mục", null, (_, _) => MoFormCon<FrmDuyet>()) { Name = "mnuDuyet" });
        mnuQuanLy.DropDownItems.Add(new ToolStripSeparator());
        mnuQuanLy.DropDownItems.Add(new ToolStripMenuItem("Thoát", null, (_, _) => Close()) { Name = "mnuThoat" });

        var mnuCascade = new ToolStripMenuItem("Xếp tầng", null, (_, _) => LayoutMdi(MdiLayout.Cascade)) { Name = "mnuCascade" };
        var mnuNgang = new ToolStripMenuItem("Xếp ngang", null, (_, _) => LayoutMdi(MdiLayout.TileHorizontal)) { Name = "mnuNgang" };
        var mnuDoc = new ToolStripMenuItem("Xếp dọc", null, (_, _) => LayoutMdi(MdiLayout.TileVertical)) { Name = "mnuDoc" };
        var mnuDongTatCa = new ToolStripMenuItem("Đóng tất cả", null, (_, _) => DongTatCa()) { Name = "mnuDongTatCa" };
        mnuCuaSo.DropDownItems.AddRange(new ToolStripItem[] { mnuCascade, mnuNgang, mnuDoc, new ToolStripSeparator(), mnuDongTatCa });
        menuStrip1.Items.AddRange(new ToolStripItem[] { mnuQuanLy, mnuCuaSo });
        menuStrip1.MdiWindowListItem = mnuCuaSo;
        Controls.Add(menuStrip1);

        lblGio.AutoSize = true;
        lblSoCuaSo.Spring = true;
        statusStrip1.Items.Add(lblGio);
        statusStrip1.Items.Add(lblSoCuaSo);
        Controls.Add(statusStrip1);
    }

    private void GanTenControl()
    {
        foreach (FieldInfo field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            if (field.GetValue(this) is Control control) control.Name = field.Name;
    }

    private void FrmMain_Load(object? sender, EventArgs e)
    {
        Properties.Settings settings = Properties.Settings.Default;
        if (settings.HasSavedWindow)
        {
            var bounds = new Rectangle(settings.Left, settings.Top, settings.Width, settings.Height);
            bool visible = Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(bounds));
            if (visible && bounds.Width > 0 && bounds.Height > 0)
            {
                WindowState = FormWindowState.Normal;
                Bounds = bounds;
                WindowState = settings.WindowState;
            }
            foreach (string child in settings.OpenChildren)
            {
                switch (child)
                {
                    case nameof(FrmSinhVien): MoFormCon<FrmSinhVien>(); break;
                    case nameof(FrmEditor): MoFormCon<FrmEditor>(); break;
                    case nameof(FrmDuyet): MoFormCon<FrmDuyet>(); break;
                }
            }
        }
        tmrDongHo_Tick(sender, e);
        CapNhatSoCuaSo();
    }

    private void FrmMain_FormClosing(object? sender, FormClosingEventArgs e)
    {
        Rectangle bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
        string[] children = MdiChildren.Select(form => form.Name).ToArray();
        Properties.Settings.Default.SaveWindow(bounds, WindowState, children);
    }

    private void MoFormCon<T>() where T : Form, new()
    {
        T? daMo = MdiChildren.OfType<T>().FirstOrDefault();
        if (daMo != null)
        {
            if (daMo.WindowState == FormWindowState.Minimized) daMo.WindowState = FormWindowState.Normal;
            daMo.Activate();
            return;
        }
        var form = new T { MdiParent = this };
        form.FormClosed += (_, _) => CapNhatSoCuaSo();
        form.Show();
        CapNhatSoCuaSo();
    }

    private void DongTatCa()
    {
        foreach (Form form in MdiChildren.ToArray()) form.Close();
        CapNhatSoCuaSo();
    }

    private void CapNhatSoCuaSo() => lblSoCuaSo.Text = $"Đang mở: {MdiChildren.Length} cửa sổ";
    private void tmrDongHo_Tick(object? sender, EventArgs e) => lblGio.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
}
