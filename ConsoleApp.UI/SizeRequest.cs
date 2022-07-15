using System.Drawing;

namespace ConsoleApp.UI
{
    public class SizeRequest
    {
        public Size Minimum
        {
            get;
            set;
        }

        public Size Request
        {
            get;
            set;
        }

        public SizeRequest(Size request)
            : this(request, request)
        {
        }

        public SizeRequest(Size request, Size minimum)
        {
            Request = request;
            Minimum = minimum;
        }
    }
}