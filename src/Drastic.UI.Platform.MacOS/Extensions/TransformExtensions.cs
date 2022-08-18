using System;
using CoreGraphics;
using Drastic.UI.Shapes;

#if __MOBILE__
namespace Drastic.UI.Platform.iOS
#else
namespace Drastic.UI.Platform.MacOS
#endif
{
    public static class TransformExtensions
	{
        public static CGAffineTransform ToCGAffineTransform(this Transform transform)
        {
            if (transform == null)
                return CGAffineTransform.MakeIdentity();

            Matrix matrix = transform.Value;

            return new CGAffineTransform(
				new nfloat(matrix.M11),
                new nfloat(matrix.M12),
                new nfloat(matrix.M21),
                new nfloat(matrix.M22),
                new nfloat(matrix.OffsetX),
                new nfloat(matrix.OffsetY));
        }
    }
}