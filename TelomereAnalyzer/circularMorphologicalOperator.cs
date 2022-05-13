

namespace TelomereAnalyzer
{
    public class circularMorphologicalOperator //: IDisposable
    {
        /*
        public Int32 _radius = 0;
        public Int32[,] pixArray;
        public circularMorphologicalOperator(Int32 iR)   // Currently cirlces, only!
        {
            _radius = iR;
            GenerateArray();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            pixArray = null;
        }
        protected bool GenerateArray()
        {
            bool success = true;
            Int32 width = _radius * 2;
            Int32 height = _radius * 2;

            Image<Gray, byte> temp = new Image<Gray, byte>(width, height, new Gray(0));
            temp.Draw(new CircleF(new PointF(_radius, _radius), (float)_radius), new Gray(255), -1);

            if (pixArray != null)
                pixArray = null;

            pixArray = new int[height, width];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    pixArray[y, x] = temp.Data[y, x, 0];


            return success;
        }

        public StructuringElementEx getElement()
        {
            StructuringElementEx structElement = new StructuringElementEx(pixArray, _radius, _radius);
            return structElement;
        }
        */
    }

}
