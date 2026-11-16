namespace Bai1;

public class SinhVien
{
    public string MaSV { get; set; } = "";
    public string HoTen { get; set; } = "";
    public string Lop { get; set; } = "";
    public double Diem { get; set; }

    public SinhVien() { }

    public SinhVien(string ma, string ten, string lop, double diem)
    {
        MaSV = ma;
        HoTen = ten;
        Lop = lop;
        Diem = diem;
    }
}
