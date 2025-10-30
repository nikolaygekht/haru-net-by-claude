using System;
using FluentAssertions;
using Haru.Types;
using Xunit;


namespace Haru.Test.Types
{
    public class HpdfMatrixTests
    {
        [Fact]
        public void TransMatrix_Identity_HasCorrectValues()
        {
            var identity = HpdfTransMatrix.Identity;

            identity.A.Should().Be(1);
            identity.B.Should().Be(0);
            identity.C.Should().Be(0);
            identity.D.Should().Be(1);
            identity.X.Should().Be(0);
            identity.Y.Should().Be(0);
        }

        [Fact]
        public void TransMatrix_CreateTranslation_SetsCorrectValues()
        {
            var matrix = HpdfTransMatrix.CreateTranslation(10, 20);

            matrix.A.Should().Be(1);
            matrix.D.Should().Be(1);
            matrix.X.Should().Be(10);
            matrix.Y.Should().Be(20);
        }

        [Fact]
        public void TransMatrix_CreateScale_SetsCorrectValues()
        {
            var matrix = HpdfTransMatrix.CreateScale(2, 3);

            matrix.A.Should().Be(2);
            matrix.D.Should().Be(3);
            matrix.X.Should().Be(0);
            matrix.Y.Should().Be(0);
        }

        [Fact]
        public void TransMatrix_CreateRotation_SetsCorrectValues()
        {
            var matrix = HpdfTransMatrix.CreateRotation((float)(Math.PI / 2)); // 90 degrees

            matrix.A.Should().BeApproximately(0, 0.0001f);
            matrix.B.Should().BeApproximately(1, 0.0001f);
            matrix.C.Should().BeApproximately(-1, 0.0001f);
            matrix.D.Should().BeApproximately(0, 0.0001f);
        }

        [Fact]
        public void TransMatrix_Equals_WorksCorrectly()
        {
            var matrix1 = new HpdfTransMatrix(1, 2, 3, 4, 5, 6);
            var matrix2 = new HpdfTransMatrix(1, 2, 3, 4, 5, 6);

            matrix1.Equals(matrix2).Should().BeTrue();
            (matrix1 == matrix2).Should().BeTrue();
        }

        [Fact]
        public void Matrix3D_Identity_HasCorrectValues()
        {
            var identity = Hpdf3DMatrix.Identity;

            identity.A.Should().Be(1);
            identity.E.Should().Be(1);
            identity.I.Should().Be(1);
            identity.Tx.Should().Be(0);
            identity.Ty.Should().Be(0);
            identity.Tz.Should().Be(0);
        }

        [Fact]
        public void Matrix3D_Constructor_SetsAllValues()
        {
            var matrix = new Hpdf3DMatrix(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);

            matrix.A.Should().Be(1);
            matrix.B.Should().Be(2);
            matrix.C.Should().Be(3);
            matrix.D.Should().Be(4);
            matrix.E.Should().Be(5);
            matrix.F.Should().Be(6);
            matrix.G.Should().Be(7);
            matrix.H.Should().Be(8);
            matrix.I.Should().Be(9);
            matrix.Tx.Should().Be(10);
            matrix.Ty.Should().Be(11);
            matrix.Tz.Should().Be(12);
        }
    }
}
