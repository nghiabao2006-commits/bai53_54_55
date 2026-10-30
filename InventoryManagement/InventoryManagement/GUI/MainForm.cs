using InventoryManagement.BUS;
using InventoryManagement.DTO;

namespace InventoryManagement.GUI;

public class MainForm : Form
{
    private readonly VatTuBUS _vatTuBus = new();
    private readonly NhaCungCapBUS _nhaCungCapBus = new();
    private readonly PhieuKhoBUS _phieuKhoBus = new();

    private readonly TextBox _txtTimVatTu = new();
    private readonly DataGridView _dgvVatTu = new();
    private readonly TextBox _txtMaVatTu = new();
    private readonly TextBox _txtTenVatTu = new();
    private readonly TextBox _txtDvt = new();
    private readonly NumericUpDown _nudSoLuongTon = new();

    private readonly TextBox _txtTimNcc = new();
    private readonly DataGridView _dgvNhaCungCap = new();
    private readonly Label _lblMaNcc = new();
    private readonly TextBox _txtTenNcc = new();
    private readonly TextBox _txtDienThoai = new();
    private readonly TextBox _txtDiaChi = new();

    private readonly ComboBox _cboLoaiPhieu = new();
    private readonly DateTimePicker _dtpNgayLap = new();
    private readonly ComboBox _cboNhaCungCapPhieu = new();
    private readonly Label _lblTongTienPhieu = new();
    private readonly ComboBox _cboVatTu = new();
    private readonly NumericUpDown _nudSoLuong = new();
    private readonly NumericUpDown _nudDonGia = new();
    private readonly DataGridView _dgvChiTietPhieu = new();
    private readonly DataGridView _dgvPhieuKho = new();
    private readonly Label _lblPhieuDaChon = new();
    private readonly DataGridView _dgvChiTietDaChon = new();

    private readonly NumericUpDown _nudNguong = new();
    private readonly DataGridView _dgvTonKho = new();
    private readonly Label _lblTongGiaTri = new();

    private readonly List<VatTuDTO> _vatTuList = new();
    private readonly List<NhaCungCapDTO> _nhaCungCapList = new();
    private readonly List<ChiTietPhieuKhoDTO> _chiTietPhieu = new();
    private readonly List<PhieuKhoDTO> _phieuKhoList = new();

    private bool _dangNapDuLieu;

    public MainForm()
    {
        Text = "Quản lý kho nguyên vật liệu";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1100, 700);
        ClientSize = new Size(1280, 800);
        WindowState = FormWindowState.Maximized;
        BackColor = UiTheme.Background;
        Font = UiTheme.BaseFont;
        BuildUi();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        ThucHien(NapDuLieuBanDau);
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = UiTheme.Background
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 84F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        root.Controls.Add(BuildHeader(), 0, 0);

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Padding = new Point(16, 6)
        };
        tabs.TabPages.Add(BuildVatTuTab());
        tabs.TabPages.Add(BuildNhaCungCapTab());
        tabs.TabPages.Add(BuildPhieuKhoTab());
        tabs.TabPages.Add(BuildTonKhoTab());
        root.Controls.Add(tabs, 0, 1);

        Controls.Add(root);
    }

    private static Panel BuildHeader()
    {
        var header = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Primary, Margin = new Padding(0) };
        header.Controls.Add(new Label
        {
            Text = "QUẢN LÝ QUY TRÌNH NHẬP - XUẤT KHO",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(24, 13)
        });
        header.Controls.Add(new Label
        {
            Text = "Bài tập 5.5  •  Kiến trúc 3 tầng DTO - DAL - BUS - GUI  •  SqlTransaction & SqlParameter",
            ForeColor = Color.FromArgb(200, 240, 235),
            Font = new Font("Segoe UI", 9.5F),
            AutoSize = true,
            Location = new Point(26, 52)
        });
        return header;
    }

    private TabPage BuildVatTuTab()
    {
        var page = new TabPage("Vật tư") { BackColor = UiTheme.Background, Padding = new Padding(10) };

        var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66F));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            BackColor = UiTheme.Surface,
            Padding = new Padding(10)
        };
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, BackColor = UiTheme.Surface };
        var lblSearch = new Label
        {
            Text = "Tìm theo mã hoặc tên:",
            AutoSize = true,
            Height = 30,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 7, 6, 0)
        };
        _txtTimVatTu.Width = 280;
        _txtTimVatTu.Height = 30;
        _txtTimVatTu.Margin = new Padding(0, 7, 6, 0);
        _txtTimVatTu.PlaceholderText = "VD: VT001 hoặc Gạo";
        _txtTimVatTu.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ThucHien(() => LoadVatTu(_txtTimVatTu.Text));
            }
        };

        var btnTim = UiTheme.CreateButton("Tìm kiếm", UiTheme.Primary, 108, 32);
        btnTim.Margin = new Padding(0, 7, 6, 0);
        btnTim.Click += (_, _) => ThucHien(() => LoadVatTu(_txtTimVatTu.Text));

        var btnTaiLai = UiTheme.CreateButton("Tải lại", UiTheme.Muted, 96, 32);
        btnTaiLai.Margin = new Padding(0, 7, 0, 0);
        btnTaiLai.Click += (_, _) =>
        {
            _txtTimVatTu.Clear();
            ThucHien(() => LoadVatTu(string.Empty));
        };

        bar.Controls.Add(lblSearch);
        bar.Controls.Add(_txtTimVatTu);
        bar.Controls.Add(btnTim);
        bar.Controls.Add(btnTaiLai);

        UiTheme.StyleGrid(_dgvVatTu);
        _dgvVatTu.Columns.AddRange(
            Col("Mã vật tư", nameof(VatTuDTO.MaVatTu), 90),
            Col("Tên vật tư", nameof(VatTuDTO.TenVatTu), 220),
            Col("ĐVT", nameof(VatTuDTO.DVT), 70),
            Col("Số lượng tồn", nameof(VatTuDTO.SoLuongTon), 105, "N2", true));
        _dgvVatTu.SelectionChanged += DgvVatTu_SelectionChanged;
        _dgvVatTu.Dock = DockStyle.Fill;

        left.Controls.Add(bar, 0, 0);
        left.Controls.Add(_dgvVatTu, 0, 1);

        _txtMaVatTu.MaxLength = 20;
        _txtTenVatTu.MaxLength = 150;
        _txtDvt.MaxLength = 20;
        _nudSoLuongTon.Minimum = 0;
        _nudSoLuongTon.Maximum = 1_000_000_000m;
        _nudSoLuongTon.DecimalPlaces = 2;
        _nudSoLuongTon.Increment = 1;

        var form = CreateFormLayout();
        AddRow(form, "Mã vật tư", _txtMaVatTu);
        AddRow(form, "Tên vật tư", _txtTenVatTu);
        AddRow(form, "Đơn vị tính", _txtDvt);
        AddRow(form, "Tồn kho", _nudSoLuongTon);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight
        };
        var btnThem = UiTheme.CreateButton("Thêm", UiTheme.Success, 74);
        btnThem.Click += (_, _) => ThucHien(ThemVatTu);
        var btnSua = UiTheme.CreateButton("Cập nhật", UiTheme.Primary, 84);
        btnSua.Click += (_, _) => ThucHien(SuaVatTu);
        var btnXoa = UiTheme.CreateButton("Xóa", UiTheme.Danger, 74);
        btnXoa.Click += (_, _) => ThucHien(XoaVatTu);
        var btnMoi = UiTheme.CreateButton("Làm mới", UiTheme.Muted, 84);
        btnMoi.Click += (_, _) => LamMoiVatTu();
        buttons.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnMoi });
        AddButtonRow(form, buttons);

        body.Controls.Add(left, 0, 0);
        body.Controls.Add(CreateEditorHost("THÔNG TIN VẬT TƯ", form), 1, 0);
        page.Controls.Add(body);
        return page;
    }

    private TabPage BuildNhaCungCapTab()
    {
        var page = new TabPage("Nhà cung cấp") { BackColor = UiTheme.Background, Padding = new Padding(10) };

        var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66F));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            BackColor = UiTheme.Surface,
            Padding = new Padding(10)
        };
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, BackColor = UiTheme.Surface };
        bar.Controls.Add(new Label
        {
            Text = "Tìm theo tên hoặc SĐT:",
            AutoSize = true,
            Height = 30,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 7, 6, 0)
        });
        _txtTimNcc.Width = 280;
        _txtTimNcc.Height = 30;
        _txtTimNcc.Margin = new Padding(0, 7, 6, 0);
        _txtTimNcc.PlaceholderText = "VD: Minh Anh hoặc 0901";
        _txtTimNcc.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ThucHien(() => LoadNhaCungCap(_txtTimNcc.Text));
            }
        };

        var btnTim = UiTheme.CreateButton("Tìm kiếm", UiTheme.Primary, 108, 32);
        btnTim.Margin = new Padding(0, 7, 6, 0);
        btnTim.Click += (_, _) => ThucHien(() => LoadNhaCungCap(_txtTimNcc.Text));

        var btnTaiLai = UiTheme.CreateButton("Tải lại", UiTheme.Muted, 96, 32);
        btnTaiLai.Margin = new Padding(0, 7, 0, 0);
        btnTaiLai.Click += (_, _) =>
        {
            _txtTimNcc.Clear();
            ThucHien(() => LoadNhaCungCap(string.Empty));
        };

        bar.Controls.Add(_txtTimNcc);
        bar.Controls.Add(btnTim);
        bar.Controls.Add(btnTaiLai);

        UiTheme.StyleGrid(_dgvNhaCungCap);
        _dgvNhaCungCap.Columns.AddRange(
            Col("Mã NCC", nameof(NhaCungCapDTO.MaNCC), 60),
            Col("Tên nhà cung cấp", nameof(NhaCungCapDTO.TenNCC), 180),
            Col("Điện thoại", nameof(NhaCungCapDTO.DienThoai), 100),
            Col("Địa chỉ", nameof(NhaCungCapDTO.DiaChi), 200));
        _dgvNhaCungCap.SelectionChanged += DgvNhaCungCap_SelectionChanged;
        _dgvNhaCungCap.Dock = DockStyle.Fill;

        left.Controls.Add(bar, 0, 0);
        left.Controls.Add(_dgvNhaCungCap, 0, 1);

        _lblMaNcc.Text = "Tự động";
        _lblMaNcc.Dock = DockStyle.Fill;
        _lblMaNcc.TextAlign = ContentAlignment.MiddleLeft;
        _lblMaNcc.Font = UiTheme.BoldFont;
        _lblMaNcc.ForeColor = UiTheme.Primary;
        _txtTenNcc.MaxLength = 150;
        _txtDienThoai.MaxLength = 15;
        _txtDiaChi.MaxLength = 255;

        var form = CreateFormLayout();
        AddRow(form, "Mã NCC", _lblMaNcc);
        AddRow(form, "Tên NCC", _txtTenNcc);
        AddRow(form, "Điện thoại", _txtDienThoai);
        AddRow(form, "Địa chỉ", _txtDiaChi);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight
        };
        var btnThem = UiTheme.CreateButton("Thêm", UiTheme.Success, 74);
        btnThem.Click += (_, _) => ThucHien(ThemNhaCungCap);
        var btnSua = UiTheme.CreateButton("Cập nhật", UiTheme.Primary, 84);
        btnSua.Click += (_, _) => ThucHien(SuaNhaCungCap);
        var btnXoa = UiTheme.CreateButton("Xóa", UiTheme.Danger, 74);
        btnXoa.Click += (_, _) => ThucHien(XoaNhaCungCap);
        var btnMoi = UiTheme.CreateButton("Làm mới", UiTheme.Muted, 84);
        btnMoi.Click += (_, _) => LamMoiNhaCungCap();
        buttons.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnMoi });
        AddButtonRow(form, buttons);

        body.Controls.Add(left, 0, 0);
        body.Controls.Add(CreateEditorHost("THÔNG TIN NHÀ CUNG CẤP", form), 1, 0);
        page.Controls.Add(body);
        return page;
    }

    private TabPage BuildPhieuKhoTab()
    {
        var page = new TabPage("Nhập / xuất kho") { BackColor = UiTheme.Background, Padding = new Padding(10) };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 58F));

        var header = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            BackColor = UiTheme.Surface,
            Padding = new Padding(10, 8, 10, 4),
            Margin = new Padding(0, 0, 0, 6)
        };

        _cboLoaiPhieu.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboLoaiPhieu.Width = 140;
        _cboLoaiPhieu.Items.AddRange(new object[] { "Nhập kho", "Xuất kho" });
        _cboLoaiPhieu.SelectedIndex = 0;
        _cboLoaiPhieu.SelectedIndexChanged += (_, _) => CapNhatTrangThaiNhaCungCap();

        _dtpNgayLap.Format = DateTimePickerFormat.Custom;
        _dtpNgayLap.CustomFormat = "dd/MM/yyyy HH:mm";
        _dtpNgayLap.Width = 190;

        _cboNhaCungCapPhieu.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboNhaCungCapPhieu.Width = 280;
        _cboNhaCungCapPhieu.DisplayMember = nameof(NhaCungCapDTO.TenNCC);
        _cboNhaCungCapPhieu.ValueMember = nameof(NhaCungCapDTO.MaNCC);

        _lblTongTienPhieu.AutoSize = true;
        _lblTongTienPhieu.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblTongTienPhieu.ForeColor = UiTheme.Accent;
        _lblTongTienPhieu.Text = "Tổng tiền: 0 đ";
        _lblTongTienPhieu.Margin = new Padding(18, 10, 0, 0);

        header.Controls.Add(FieldLabel("Loại phiếu"));
        header.Controls.Add(_cboLoaiPhieu);
        header.Controls.Add(FieldLabel("Ngày lập"));
        header.Controls.Add(_dtpNgayLap);
        header.Controls.Add(FieldLabel("Nhà cung cấp"));
        header.Controls.Add(_cboNhaCungCapPhieu);
        header.Controls.Add(_lblTongTienPhieu);

        var lineBar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            BackColor = UiTheme.Surface,
            Padding = new Padding(10, 8, 10, 4)
        };

        _cboVatTu.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboVatTu.Width = 270;
        _cboVatTu.DisplayMember = nameof(VatTuDTO.HienThi);
        _cboVatTu.ValueMember = nameof(VatTuDTO.MaVatTu);

        _nudSoLuong.Minimum = 0.01m;
        _nudSoLuong.Maximum = 1_000_000_000m;
        _nudSoLuong.DecimalPlaces = 2;
        _nudSoLuong.Value = 1;
        _nudSoLuong.Width = 100;

        _nudDonGia.Minimum = 0;
        _nudDonGia.Maximum = 1_000_000_000_000m;
        _nudDonGia.DecimalPlaces = 2;
        _nudDonGia.Width = 120;

        var btnThemDong = UiTheme.CreateButton("Thêm dòng", UiTheme.Primary, 104);
        btnThemDong.Click += (_, _) => ThucHien(ThemDongChiTiet);
        var btnXoaDong = UiTheme.CreateButton("Xóa dòng", UiTheme.Danger, 100);
        btnXoaDong.Click += (_, _) => ThucHien(XoaDongChiTiet);
        var btnLuu = UiTheme.CreateButton("Lưu phiếu kho", UiTheme.Success, 125);
        btnLuu.Click += (_, _) => ThucHien(LuuPhieuKho);
        var btnMoiPhieu = UiTheme.CreateButton("Phiếu mới", UiTheme.Muted, 102);
        btnMoiPhieu.Click += (_, _) => LamMoiPhieuKho();

        lineBar.Controls.Add(FieldLabel("Vật tư"));
        lineBar.Controls.Add(_cboVatTu);
        lineBar.Controls.Add(FieldLabel("Số lượng"));
        lineBar.Controls.Add(_nudSoLuong);
        lineBar.Controls.Add(FieldLabel("Đơn giá"));
        lineBar.Controls.Add(_nudDonGia);
        lineBar.Controls.Add(btnThemDong);
        lineBar.Controls.Add(btnXoaDong);
        lineBar.Controls.Add(btnLuu);
        lineBar.Controls.Add(btnMoiPhieu);

        UiTheme.StyleGrid(_dgvChiTietPhieu);
        _dgvChiTietPhieu.Columns.AddRange(
            Col("Mã VT", nameof(ChiTietPhieuKhoDTO.MaVatTu), 80),
            Col("Tên vật tư", nameof(ChiTietPhieuKhoDTO.TenVatTu), 200),
            Col("ĐVT", nameof(ChiTietPhieuKhoDTO.DVT), 60),
            Col("Số lượng", nameof(ChiTietPhieuKhoDTO.SoLuong), 80, "N2", true),
            Col("Đơn giá", nameof(ChiTietPhieuKhoDTO.DonGia), 100, "N0", true),
            Col("Thành tiền", nameof(ChiTietPhieuKhoDTO.ThanhTien), 110, "N0", true));

        UiTheme.StyleGrid(_dgvPhieuKho);
        _dgvPhieuKho.Columns.AddRange(
            Col("Mã phiếu", nameof(PhieuKhoDTO.MaPhieu), 60),
            Col("Loại phiếu", nameof(PhieuKhoDTO.LoaiPhieuHienThi), 90),
            Col("Ngày lập", nameof(PhieuKhoDTO.NgayLap), 125, "dd/MM/yyyy HH:mm"),
            Col("Nhà cung cấp", nameof(PhieuKhoDTO.TenNCC), 170),
            Col("Tổng tiền", nameof(PhieuKhoDTO.TongTien), 110, "N0", true));
        _dgvPhieuKho.SelectionChanged += DgvPhieuKho_SelectionChanged;

        UiTheme.StyleGrid(_dgvChiTietDaChon);
        _dgvChiTietDaChon.Columns.AddRange(
            Col("Mã VT", nameof(ChiTietPhieuKhoDTO.MaVatTu), 80),
            Col("Tên vật tư", nameof(ChiTietPhieuKhoDTO.TenVatTu), 200),
            Col("ĐVT", nameof(ChiTietPhieuKhoDTO.DVT), 60),
            Col("Số lượng", nameof(ChiTietPhieuKhoDTO.SoLuong), 80, "N2", true),
            Col("Đơn giá", nameof(ChiTietPhieuKhoDTO.DonGia), 100, "N0", true),
            Col("Thành tiền", nameof(ChiTietPhieuKhoDTO.ThanhTien), 110, "N0", true));

        var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
        bottom.Controls.Add(CreateGroup("Phiếu kho đã lập", _dgvPhieuKho), 0, 0);

        var savedHost = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, BackColor = UiTheme.Surface };
        savedHost.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        savedHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _lblPhieuDaChon.Text = "Chọn một phiếu ở danh sách bên trái để xem chi tiết.";
        _lblPhieuDaChon.Dock = DockStyle.Fill;
        _lblPhieuDaChon.TextAlign = ContentAlignment.MiddleLeft;
        _lblPhieuDaChon.Font = UiTheme.SmallFont;
        _lblPhieuDaChon.ForeColor = UiTheme.Muted;
        _dgvChiTietDaChon.Dock = DockStyle.Fill;
        savedHost.Controls.Add(_lblPhieuDaChon, 0, 0);
        savedHost.Controls.Add(_dgvChiTietDaChon, 0, 1);
        bottom.Controls.Add(CreateGroup("Chi tiết phiếu đã chọn", savedHost), 1, 0);

        layout.Controls.Add(header, 0, 0);
        layout.Controls.Add(lineBar, 0, 1);
        layout.Controls.Add(CreateGroup("Chi tiết phiếu đang lập", _dgvChiTietPhieu), 0, 2);
        layout.Controls.Add(bottom, 0, 3);

        page.Controls.Add(layout);
        return page;
    }

    private TabPage BuildTonKhoTab()
    {
        var page = new TabPage("Tồn kho & báo cáo") { BackColor = UiTheme.Background, Padding = new Padding(10) };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var bar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            BackColor = UiTheme.Surface,
            Padding = new Padding(10, 8, 10, 4)
        };

        _nudNguong.Minimum = 0;
        _nudNguong.Maximum = 1_000_000;
        _nudNguong.DecimalPlaces = 2;
        _nudNguong.Value = 10;
        _nudNguong.Width = 100;

        var btnLoc = UiTheme.CreateButton("Lọc tồn thấp", UiTheme.Accent, 125);
        btnLoc.Click += (_, _) => ThucHien(() => LoadTonKho(true));
        var btnTatCa = UiTheme.CreateButton("Xem tất cả", UiTheme.Muted, 112);
        btnTatCa.Click += (_, _) => ThucHien(() => LoadTonKho(false));
        var btnTinhTong = UiTheme.CreateButton("Tính tổng giá trị", UiTheme.Success, 148);
        btnTinhTong.Click += (_, _) => ThucHien(() => LoadTonKho(false));

        _lblTongGiaTri.AutoSize = true;
        _lblTongGiaTri.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        _lblTongGiaTri.ForeColor = UiTheme.Primary;
        _lblTongGiaTri.Text = "Tổng giá trị tồn kho: 0 đ";
        _lblTongGiaTri.Margin = new Padding(20, 9, 0, 0);

        bar.Controls.Add(FieldLabel("Ngưỡng an toàn"));
        bar.Controls.Add(_nudNguong);
        bar.Controls.Add(btnLoc);
        bar.Controls.Add(btnTatCa);
        bar.Controls.Add(btnTinhTong);
        bar.Controls.Add(_lblTongGiaTri);

        UiTheme.StyleGrid(_dgvTonKho);
        _dgvTonKho.Columns.AddRange(
            Col("Mã VT", nameof(VatTuDTO.MaVatTu), 80),
            Col("Tên vật tư", nameof(VatTuDTO.TenVatTu), 240),
            Col("ĐVT", nameof(VatTuDTO.DVT), 70),
            Col("Số lượng tồn", nameof(VatTuDTO.SoLuongTon), 110, "N2", true));
        _dgvTonKho.CellFormatting += DgvTonKho_CellFormatting;

        layout.Controls.Add(bar, 0, 0);
        layout.Controls.Add(CreateGroup("Danh sách tồn kho", _dgvTonKho), 0, 1);
        page.Controls.Add(layout);
        return page;
    }

    private static TableLayoutPanel CreateEditorHost(string title, Control form)
    {
        var host = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = UiTheme.Surface,
            Padding = new Padding(16),
            Margin = new Padding(8, 0, 0, 0)
        };
        host.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        host.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        host.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = UiTheme.TitleFont,
            ForeColor = UiTheme.Primary
        }, 0, 0);
        form.Dock = DockStyle.Top;
        host.Controls.Add(form, 0, 1);
        return host;
    }

    private static TableLayoutPanel CreateFormLayout()
    {
        var form = new TableLayoutPanel
        {
            ColumnCount = 2,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowOnly,
            Dock = DockStyle.Top,
            BackColor = UiTheme.Surface
        };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        return form;
    }

    private static void AddRow(TableLayoutPanel form, string label, Control input)
    {
        var row = form.RowCount;
        form.RowCount = row + 1;
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        form.Controls.Add(new Label
        {
            Text = label,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = UiTheme.BaseFont,
            ForeColor = UiTheme.Muted,
            Margin = new Padding(0, 0, 6, 6)
        }, 0, row);
        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(0, 3, 0, 3);
        form.Controls.Add(input, 1, row);
    }

    private static void AddButtonRow(TableLayoutPanel form, Control buttons)
    {
        var row = form.RowCount;
        form.RowCount = row + 1;
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
        buttons.Dock = DockStyle.Fill;
        form.Controls.Add(buttons, 0, row);
        form.SetColumnSpan(buttons, 2);
    }

    private static GroupBox CreateGroup(string title, Control content)
    {
        var group = new GroupBox
        {
            Text = title,
            Dock = DockStyle.Fill,
            Font = UiTheme.BoldFont,
            ForeColor = UiTheme.PrimaryDark,
            Padding = new Padding(10, 6, 10, 10),
            Margin = new Padding(0, 6, 0, 0)
        };
        content.Dock = DockStyle.Fill;
        group.Controls.Add(content);
        return group;
    }

    private static Label FieldLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Height = 30,
        TextAlign = ContentAlignment.MiddleLeft,
        Font = UiTheme.BaseFont,
        ForeColor = UiTheme.Muted,
        Margin = new Padding(4, 10, 4, 0)
    };

    private static DataGridViewTextBoxColumn Col(
        string header,
        string property,
        int weight,
        string? format = null,
        bool right = false)
    {
        var column = new DataGridViewTextBoxColumn
        {
            HeaderText = header,
            DataPropertyName = property,
            FillWeight = weight,
            MinimumWidth = 55,
            SortMode = DataGridViewColumnSortMode.NotSortable
        };
        if (format is not null)
        {
            column.DefaultCellStyle.Format = format;
        }

        if (right)
        {
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        return column;
    }

    private void BindGrid(DataGridView grid, object? data)
    {
        _dangNapDuLieu = true;
        grid.DataSource = null;
        grid.DataSource = data;
        grid.ClearSelection();
        _dangNapDuLieu = false;
    }

    private void NapDuLieuBanDau()
    {
        LoadVatTu(string.Empty);
        LoadNhaCungCap(string.Empty);
        LoadVouchers();
        LoadTonKho(false);
    }

    private void LoadVatTu(string? keyword)
    {
        var data = _vatTuBus.Search(keyword);
        _vatTuList.Clear();
        _vatTuList.AddRange(data);
        BindGrid(_dgvVatTu, data);

        var selectedMa = (_cboVatTu.SelectedItem as VatTuDTO)?.MaVatTu;
        var all = _vatTuBus.Search(null);
        _cboVatTu.DataSource = null;
        _cboVatTu.DataSource = all;
        if (selectedMa is not null)
        {
            for (var i = 0; i < _cboVatTu.Items.Count; i++)
            {
                if (_cboVatTu.Items[i] is VatTuDTO item &&
                    item.MaVatTu.Equals(selectedMa, StringComparison.OrdinalIgnoreCase))
                {
                    _cboVatTu.SelectedIndex = i;
                    break;
                }
            }
        }
    }

    private void LoadNhaCungCap(string? keyword)
    {
        var data = _nhaCungCapBus.Search(keyword);
        BindGrid(_dgvNhaCungCap, data);

        var selectedMa = (_cboNhaCungCapPhieu.SelectedItem as NhaCungCapDTO)?.MaNCC;
        var all = _nhaCungCapBus.Search(null);
        _nhaCungCapList.Clear();
        _nhaCungCapList.AddRange(all);
        _cboNhaCungCapPhieu.DataSource = null;
        _cboNhaCungCapPhieu.DataSource = _nhaCungCapList.ToList();
        if (selectedMa.HasValue)
        {
            for (var i = 0; i < _cboNhaCungCapPhieu.Items.Count; i++)
            {
                if (_cboNhaCungCapPhieu.Items[i] is NhaCungCapDTO item && item.MaNCC == selectedMa.Value)
                {
                    _cboNhaCungCapPhieu.SelectedIndex = i;
                    break;
                }
            }
        }

        CapNhatTrangThaiNhaCungCap();
    }

    private void LoadVouchers()
    {
        _phieuKhoList.Clear();
        _phieuKhoList.AddRange(_phieuKhoBus.GetAll());

        var selectedMa = (_dgvPhieuKho.CurrentRow?.DataBoundItem as PhieuKhoDTO)?.MaPhieu;
        BindGrid(_dgvPhieuKho, _phieuKhoList.ToList());

        if (_dgvPhieuKho.Rows.Count == 0)
        {
            BindGrid(_dgvChiTietDaChon, null);
            _lblPhieuDaChon.Text = "Chưa có phiếu kho nào.";
            return;
        }

        var targetIndex = 0;
        if (selectedMa.HasValue)
        {
            for (var i = 0; i < _dgvPhieuKho.Rows.Count; i++)
            {
                if (_dgvPhieuKho.Rows[i].DataBoundItem is PhieuKhoDTO item && item.MaPhieu == selectedMa.Value)
                {
                    targetIndex = i;
                    break;
                }
            }
        }

        _dgvPhieuKho.Rows[targetIndex].Selected = true;
        _dgvPhieuKho.CurrentCell = _dgvPhieuKho.Rows[targetIndex].Cells[0];
    }

    private void LoadTonKho(bool onlyLowStock)
    {
        var threshold = _nudNguong.Value;
        var data = onlyLowStock ? _vatTuBus.GetLowStock(threshold) : _vatTuBus.Search(null);
        BindGrid(_dgvTonKho, data);

        foreach (DataGridViewRow row in _dgvTonKho.Rows)
        {
            if (row.DataBoundItem is VatTuDTO vatTu && vatTu.SoLuongTon < threshold)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                row.DefaultCellStyle.ForeColor = UiTheme.Danger;
            }
        }

        _lblTongGiaTri.Text = "Tổng giá trị tồn kho: " +
            _vatTuBus.GetTotalInventoryValue().ToString("N0") + " đ";
    }

    private void DgvTonKho_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 ||
            e.CellStyle is null ||
            _dgvTonKho.Rows[e.RowIndex].DataBoundItem is not VatTuDTO vatTu ||
            vatTu.SoLuongTon >= _nudNguong.Value)
        {
            return;
        }

        e.CellStyle.BackColor = Color.FromArgb(255, 235, 238);
        e.CellStyle.ForeColor = UiTheme.Danger;
    }

    private void DgvVatTu_SelectionChanged(object? sender, EventArgs e)
    {
        if (_dangNapDuLieu || _dgvVatTu.CurrentRow?.DataBoundItem is not VatTuDTO vatTu)
        {
            return;
        }

        _txtMaVatTu.Text = vatTu.MaVatTu;
        _txtMaVatTu.ReadOnly = true;
        _txtTenVatTu.Text = vatTu.TenVatTu;
        _txtDvt.Text = vatTu.DVT;
        _nudSoLuongTon.Value = Math.Clamp(
            vatTu.SoLuongTon, _nudSoLuongTon.Minimum, _nudSoLuongTon.Maximum);
    }

    private void DgvNhaCungCap_SelectionChanged(object? sender, EventArgs e)
    {
        if (_dangNapDuLieu || _dgvNhaCungCap.CurrentRow?.DataBoundItem is not NhaCungCapDTO nhaCungCap)
        {
            return;
        }

        _lblMaNcc.Text = nhaCungCap.MaNCC.ToString();
        _txtTenNcc.Text = nhaCungCap.TenNCC;
        _txtDienThoai.Text = nhaCungCap.DienThoai ?? string.Empty;
        _txtDiaChi.Text = nhaCungCap.DiaChi ?? string.Empty;
    }

    private void DgvPhieuKho_SelectionChanged(object? sender, EventArgs e)
    {
        if (_dangNapDuLieu || _dgvPhieuKho.CurrentRow?.DataBoundItem is not PhieuKhoDTO phieu)
        {
            return;
        }

        try
        {
            BindGrid(_dgvChiTietDaChon, _phieuKhoBus.GetDetails(phieu.MaPhieu));
            _lblPhieuDaChon.Text =
                $"Phiếu #{phieu.MaPhieu} • {phieu.LoaiPhieuHienThi} • {phieu.NgayLap:dd/MM/yyyy HH:mm}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ThemVatTu()
    {
        var vatTu = ReadVatTuForm(_txtMaVatTu.Text);
        _vatTuBus.Save(vatTu, true);
        MessageBox.Show(this, "Đã thêm vật tư.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LamMoiVatTu();
        LoadVatTu(_txtTimVatTu.Text);
        LoadTonKho(false);
    }

    private void SuaVatTu()
    {
        var selected = GetSelected<VatTuDTO>(_dgvVatTu)
            ?? throw new ArgumentException("Vui lòng chọn vật tư cần cập nhật.");
        var vatTu = ReadVatTuForm(selected.MaVatTu);
        _vatTuBus.Save(vatTu, false);
        MessageBox.Show(this, "Đã cập nhật vật tư.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LoadVatTu(_txtTimVatTu.Text);
        LoadTonKho(false);
    }

    private void XoaVatTu()
    {
        var selected = GetSelected<VatTuDTO>(_dgvVatTu)
            ?? throw new ArgumentException("Vui lòng chọn vật tư cần xóa.");
        var answer = MessageBox.Show(
            this,
            $"Xóa vật tư '{selected.TenVatTu}'?",
            "Xác nhận",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (answer != DialogResult.Yes)
        {
            return;
        }

        _vatTuBus.Delete(selected.MaVatTu);
        LamMoiVatTu();
        LoadVatTu(_txtTimVatTu.Text);
        LoadTonKho(false);
    }

    private VatTuDTO ReadVatTuForm(string maVatTu) => new()
    {
        MaVatTu = maVatTu,
        TenVatTu = _txtTenVatTu.Text,
        DVT = _txtDvt.Text,
        SoLuongTon = _nudSoLuongTon.Value
    };

    private void LamMoiVatTu()
    {
        _dangNapDuLieu = true;
        _dgvVatTu.ClearSelection();
        _dangNapDuLieu = false;
        _txtMaVatTu.Clear();
        _txtMaVatTu.ReadOnly = false;
        _txtTenVatTu.Clear();
        _txtDvt.Clear();
        _nudSoLuongTon.Value = 0;
        _txtMaVatTu.Focus();
    }

    private void ThemNhaCungCap()
    {
        var nhaCungCap = new NhaCungCapDTO
        {
            TenNCC = _txtTenNcc.Text,
            DienThoai = _txtDienThoai.Text,
            DiaChi = _txtDiaChi.Text
        };
        _nhaCungCapBus.Save(nhaCungCap, true);
        MessageBox.Show(this, "Đã thêm nhà cung cấp.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LamMoiNhaCungCap();
        LoadNhaCungCap(_txtTimNcc.Text);
    }

    private void SuaNhaCungCap()
    {
        var selected = GetSelected<NhaCungCapDTO>(_dgvNhaCungCap)
            ?? throw new ArgumentException("Vui lòng chọn nhà cung cấp cần cập nhật.");
        var nhaCungCap = new NhaCungCapDTO
        {
            MaNCC = selected.MaNCC,
            TenNCC = _txtTenNcc.Text,
            DienThoai = _txtDienThoai.Text,
            DiaChi = _txtDiaChi.Text
        };
        _nhaCungCapBus.Save(nhaCungCap, false);
        MessageBox.Show(this, "Đã cập nhật nhà cung cấp.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LoadNhaCungCap(_txtTimNcc.Text);
    }

    private void XoaNhaCungCap()
    {
        var selected = GetSelected<NhaCungCapDTO>(_dgvNhaCungCap)
            ?? throw new ArgumentException("Vui lòng chọn nhà cung cấp cần xóa.");
        var answer = MessageBox.Show(
            this,
            $"Xóa nhà cung cấp '{selected.TenNCC}'?",
            "Xác nhận",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (answer != DialogResult.Yes)
        {
            return;
        }

        _nhaCungCapBus.Delete(selected.MaNCC);
        LamMoiNhaCungCap();
        LoadNhaCungCap(_txtTimNcc.Text);
    }

    private void LamMoiNhaCungCap()
    {
        _dangNapDuLieu = true;
        _dgvNhaCungCap.ClearSelection();
        _dangNapDuLieu = false;
        _lblMaNcc.Text = "Tự động";
        _txtTenNcc.Clear();
        _txtDienThoai.Clear();
        _txtDiaChi.Clear();
        _txtTenNcc.Focus();
    }

    private void CapNhatTrangThaiNhaCungCap()
    {
        var isNhap = _cboLoaiPhieu.SelectedIndex == 0;
        _cboNhaCungCapPhieu.Enabled = isNhap;
        if (!isNhap)
        {
            _cboNhaCungCapPhieu.SelectedIndex = -1;
        }
        else if (_cboNhaCungCapPhieu.SelectedIndex < 0 && _cboNhaCungCapPhieu.Items.Count > 0)
        {
            _cboNhaCungCapPhieu.SelectedIndex = 0;
        }
    }

    private void ThemDongChiTiet()
    {
        if (_cboVatTu.SelectedItem is not VatTuDTO vatTu)
        {
            throw new ArgumentException("Vui lòng chọn vật tư.");
        }

        var soLuong = _nudSoLuong.Value;
        if (soLuong <= 0)
        {
            throw new ArgumentException("Số lượng phải lớn hơn 0.");
        }

        var donGia = _nudDonGia.Value;
        var existing = _chiTietPhieu.FirstOrDefault(
            ct => ct.MaVatTu.Equals(vatTu.MaVatTu, StringComparison.OrdinalIgnoreCase));
        if (existing is null)
        {
            _chiTietPhieu.Add(new ChiTietPhieuKhoDTO
            {
                MaVatTu = vatTu.MaVatTu,
                TenVatTu = vatTu.TenVatTu,
                DVT = vatTu.DVT,
                SoLuong = soLuong,
                DonGia = donGia
            });
        }
        else
        {
            existing.SoLuong += soLuong;
            existing.DonGia = donGia;
        }

        RefreshChiTietPhieu();
    }

    private void XoaDongChiTiet()
    {
        if (_dgvChiTietPhieu.CurrentRow?.DataBoundItem is not ChiTietPhieuKhoDTO chiTiet)
        {
            throw new ArgumentException("Vui lòng chọn dòng chi tiết cần xóa.");
        }

        _chiTietPhieu.RemoveAll(
            ct => ct.MaVatTu.Equals(chiTiet.MaVatTu, StringComparison.OrdinalIgnoreCase));
        RefreshChiTietPhieu();
    }

    private void LuuPhieuKho()
    {
        var isNhap = _cboLoaiPhieu.SelectedIndex == 0;
        var phieu = new PhieuKhoDTO
        {
            LoaiPhieu = isNhap ? "NHAP" : "XUAT",
            NgayLap = _dtpNgayLap.Value,
            MaNCC = isNhap && _cboNhaCungCapPhieu.SelectedItem is NhaCungCapDTO nhaCungCap
                ? nhaCungCap.MaNCC
                : null,
            ChiTiet = _chiTietPhieu.Select(ct => new ChiTietPhieuKhoDTO
            {
                MaVatTu = ct.MaVatTu,
                TenVatTu = ct.TenVatTu,
                DVT = ct.DVT,
                SoLuong = ct.SoLuong,
                DonGia = ct.DonGia
            }).ToList()
        };

        var maPhieu = _phieuKhoBus.Create(phieu);
        MessageBox.Show(
            this,
            $"Đã lưu phiếu #{maPhieu} - {phieu.LoaiPhieuHienThi}.",
            "Thành công",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        LamMoiPhieuKho();
        LoadVatTu(_txtTimVatTu.Text);
        LoadVouchers();
        LoadTonKho(false);
    }

    private void LamMoiPhieuKho()
    {
        _chiTietPhieu.Clear();
        _dtpNgayLap.Value = DateTime.Now;
        _cboLoaiPhieu.SelectedIndex = 0;
        _nudSoLuong.Value = 1;
        _nudDonGia.Value = 0;
        RefreshChiTietPhieu();
        CapNhatTrangThaiNhaCungCap();
        BindGrid(_dgvChiTietDaChon, null);
        _lblPhieuDaChon.Text = "Chọn một phiếu ở danh sách bên trái để xem chi tiết.";
    }

    private void RefreshChiTietPhieu()
    {
        BindGrid(_dgvChiTietPhieu, _chiTietPhieu.ToList());
        _lblTongTienPhieu.Text = "Tổng tiền: " +
            _chiTietPhieu.Sum(ct => ct.ThanhTien).ToString("N0") + " đ";
    }

    private static T? GetSelected<T>(DataGridView grid) where T : class
        => grid.CurrentRow?.DataBoundItem as T;

    private void ThucHien(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
