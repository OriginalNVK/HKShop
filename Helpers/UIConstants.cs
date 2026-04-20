namespace HKShop.Helpers;

public static class UIConstants
{
    public static class Brand
    {
        public const string Name = "HK SHOP";
        public const string CustomerTitle = "HK Shop";
        public const string AdminTitle = "HK Shop - Admin";
    }

    public static class Contact
    {
        public const string Phone = "+8485-898-1798";
        public const string Email = "nvk.work@outlook.com.vn";
        public const string CompanyName = "HK Limited Company";
    }

    public static class Navigation
    {
        public const string HomePage = "Home Page";
        public const string VisitStore = "Visit Store";
        public const string Wishlist = "Wishlist";
        public const string Menu = "Menu";
        public const string Account = "Tai khoan";
        public const string Login = "Dang nhap";
        public const string Register = "Dang ky";
        public const string Logout = "Dang xuat";
    }

    public static class Auth
    {
        public const string LoginTitle = "Login";
        public const string RegisterTitle = "Register Member";
        public const string LoginButton = "Login";
        public const string RegisterButton = "Register";
        public const string ForgotPasswordButton = "Forgot Password";
        public const string Gender = "Gioi tinh";
        public const string Male = "Nam";
        public const string Female = "Nu";
        public const string ImageLabel = "Image";
    }

    public static class Product
    {
        public const string AddToCart = "Add To Cart";
        public const string AddToWishlist = "Add to Wishlist";
        public const string Compare = "Compare";
        public const string ViewDetails = "View Details";
        public const string NewBadge = "NEW";
        public const string RelatedProducts = "Related Products";
        public const string Qty = "Qty";
    }

    public static class Cart
    {
        public const string CartLabel = "Cart";
        public const string EmptyCart = "Empty Cart";
        public const string ViewCart = "View Cart";
        public const string Checkout = "Checkout";
        public const string ItemSelectedSuffix = "Item(s) selected";
        public const string Subtotal = "SUBTOTAL";
    }

    public static class Footer
    {
        public const string AboutUs = "ABOUT US";
        public const string AboutDescription = "HK Shop - Your One-Stop Technology Store";
        public const string Categories = "CATEGORIES";
        public const string Information = "INFORMATION";
        public const string Services = "SERVICES";
    }

    public static readonly IReadOnlyList<FooterLink> FooterCategoryLinks = new List<FooterLink>
    {
        new("Visit Store", "/HangHoa"),
        new("Laptops", "/HangHoa?MaLoai=1001"),
        new("Smartphones", "/HangHoa?MaLoai=1003"),
        new("Cameras", "/HangHoa?MaLoai=1002"),
        new("Watches", "/HangHoa?MaLoai=1000")
    };

    public static readonly IReadOnlyList<FooterLink> FooterInformationLinks = new List<FooterLink>
    {
        new("About Us", "/blank"),
        new("Contact Us", "/blank"),
        new("Privacy Policy", "/blank"),
        new("Terms & Conditions", "/blank")
    };

    public static readonly IReadOnlyList<FooterLink> FooterServiceLinks = new List<FooterLink>
    {
        new("My Account", "/Profile"),
        new("Shopping Cart", "/Cart"),
        new("Wishlist", "/blank")
    };
}

public sealed class FooterLink
{
    public FooterLink(string text, string href)
    {
        Text = text;
        Href = href;
    }

    public string Text { get; }

    public string Href { get; }
}
