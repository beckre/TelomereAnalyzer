using System;
using System.Drawing;


namespace TelomereAnalyzer
{
    public class VesselAnalysisClass : IDisposable
    {
        ElmiWood _parentForm = null;
        public VesselClass[] _vesselInTissue = null;

        protected enum ClusterMergePos : int
        {
            UNKNOWN = 0,
            HIT_AT_HEAD = 1,
            HIT_AT_TAIL = 2
        }

        public struct VesselCluster
        {
            public VesselClass[] vessels;
            public Int32 id;
            public Int32 globalCenterY;
            public Int32 globalHeight;
            public bool saveCluster;           // Large supercluster: indicating for being well analyzed
            public bool mistrustedCluster;     // Large supercluster: indicating for being not well fitting to overall data
            public spotField[] testSpots;
        }

        public VesselCluster[] _vesselCluster = null;
        StochasticsClass _stochastics = null;

        public struct spotField
        {
            public Int32 Xc;   // Gravity center
            public Int32 Yc;
            public Int32 Xo;   // left upper
            public Int32 Yo;
            public Int32 X1;   // right lower
            public Int32 Y1;
        }

        public struct StochasticalVesselGeometry
        {
            public Int64 vesselsFound;
            public Double meanArea;
            public Double meanWidth;
            public Double meanHeight;
            public Double stdArea;
            public Double stdWidth;
            public Double stdHeight;
            public bool calculated;
        }

        public StochasticalVesselGeometry _normalizedVesselGeometry;
        public Double _dHorizontalToleranceMultiplicator = 1.0;
        public Double _dVerticalToleranceMultiplicator = 1.0;

        #region Construction, initialization, destruction --------------------------------------------------------------------------
        public VesselAnalysisClass(ElmiWood parentForm, ref VesselClass[] vesselsToBeAnalysed)
        {
            _parentForm = parentForm;
            _vesselInTissue = vesselsToBeAnalysed;
            InitializeMembers();
        }

        protected void InitializeMembers()
        {
            _normalizedVesselGeometry.vesselsFound = 0;
            _normalizedVesselGeometry.meanArea = -1.0;
            _normalizedVesselGeometry.meanWidth = -1.0;
            _normalizedVesselGeometry.meanHeight = -1.0;
            _normalizedVesselGeometry.stdArea = -1.0;
            _normalizedVesselGeometry.stdWidth = -1.0;
            _normalizedVesselGeometry.stdHeight = -1.0;
            _normalizedVesselGeometry.calculated = false;
            _stochastics = new StochasticsClass();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (_stochastics != null)
                _stochastics.Dispose();
            _stochastics = null;
        }

        ~VesselAnalysisClass()
        {
            Dispose();
        }
        #endregion (construction, initialization, destruction)

        #region Interface ----------------------------------------------------------------------------------------------------------
        public bool CalculateStandardValues()
        {
            _normalizedVesselGeometry.calculated = false;

            if (_vesselInTissue == null)
                return false;

            _normalizedVesselGeometry.vesselsFound = _vesselInTissue.Length;

            if (CalculateMeanAreaValues() == false)
                return false;

            if (CalculateMeanBoundingBox() == false)
                return false;

            _normalizedVesselGeometry.calculated = true;
            return true;
        }

        public bool ClusterVessels(Double horTolerance, Double verTolerance)
        {
            bool success = true;
            InitClusterAnalysis();
            //Im Moment werden 1.70 übergeben als Default Wert
            _dHorizontalToleranceMultiplicator = horTolerance;
            _dVerticalToleranceMultiplicator = verTolerance;

            success = DoClusterAnalysis();

            if (success == false)
                return false;

            //_parentForm.labOutPut.Text = "Cluster analysis: Marking unsafe clusters...";
            Console.WriteLine("Cluster analysis: Marking unsafe clusters...");
            System.Windows.Forms.Application.DoEvents();

            if (success == true)
                MarkUnsafeClusters();

            //_parentForm.labOutPut.Text = "Super cluster analysis...";
            Console.WriteLine("Super cluster analysis...");
            System.Windows.Forms.Application.DoEvents();

            if (success == true)
                success = SuperClusterAnalysis();

            //_parentForm.labOutPut.Text = "Marking mistrusted clusters...";
            Console.WriteLine("Marking mistrusted clusters...");
            System.Windows.Forms.Application.DoEvents();

            if (success == true)
                MarkMistrustedClusters();

            return success;
        }

        public void GetAmountClustersFound(ref Int32 safeClusters, ref Int32 unsafeClusters)
        {
            safeClusters = 0;
            unsafeClusters = 0;

            if (_vesselCluster == null)
                return;

            for (Int32 cV = 0; cV < _vesselCluster.Length; cV++)
            {
                if (_vesselCluster[cV].saveCluster == false)
                    unsafeClusters++;

                if (_vesselCluster[cV].saveCluster == true)
                    safeClusters++;
            }
        }

        protected void MarkUnsafeClusters()
        {
            if (_vesselCluster == null)
                return;

            if (_vesselCluster.Length <= 0)
                return;

            Int32 MinValue = 2;

            for (Int32 cV = 0; cV < _vesselCluster.Length; cV++)
            {
                if (_vesselCluster[cV].vessels == null)
                    _vesselCluster[cV].saveCluster = false;
                else
                {
                    if (_vesselCluster[cV].vessels.Length < MinValue)   // All clusters with less than 5 vessels are unsafe
                        _vesselCluster[cV].saveCluster = false;
                    else
                        _vesselCluster[cV].saveCluster = true;
                }
            }
        }

        protected void MarkMistrustedClusters()
        {
            if (_vesselCluster == null)
                return;

            if (_vesselCluster.Length <= 0)
                return;

            Int32 relevantItems = 0;

            for (Int32 cV = 0; cV < _vesselCluster.Length; cV++)
            {
                if (_vesselCluster[cV].saveCluster == true)
                    relevantItems++;
            }

            Int32[] vesselsInCluster = new Int32[relevantItems];
            Int32 c = 0;

            for (Int32 cV = 0; cV < _vesselCluster.Length; cV++)
            {
                if (_vesselCluster[cV].saveCluster == true)
                    vesselsInCluster[c++] = _vesselCluster[cV].vessels.Length;
            }

            Array.Sort(vesselsInCluster);

            Int32 Median = vesselsInCluster[vesselsInCluster.Length / 2]; //Exception System.IndexOutOfRangeException: Der Index war außerhalb des Arraybereichs
            Int32 percentil = Convert.ToInt32(Median * 0.1);
            Int32 confidence = 2;

            if (_vesselCluster.Length - 1 < confidence)
                return;

            Int32 minCloud = vesselsInCluster[0] + percentil;
            Int32 maxCloud = vesselsInCluster[relevantItems - 2] + percentil;

            for (Int32 cV = 0; cV < _vesselCluster.Length; cV++)
            {
                if (_vesselCluster[cV].vessels == null)
                    _vesselCluster[cV].mistrustedCluster = true;
                else
                {
                    if (_vesselCluster[cV].vessels.Length < minCloud)   // All clusters with less than 5 vessels are unsafe
                        _vesselCluster[cV].mistrustedCluster = true;
                    else
                    {
                        if (_vesselCluster[cV].vessels.Length > maxCloud)   // All clusters with less than 5 vessels are unsafe
                            _vesselCluster[cV].mistrustedCluster = true;
                        else
                            _vesselCluster[cV].mistrustedCluster = false;
                    }
                }
            }
        }

        public void GetResult(ref StochasticalVesselGeometry normalizedVesselGeometry)
        {
            normalizedVesselGeometry = _normalizedVesselGeometry;
        }

        public bool GetVesselClusters(ref VesselAnalysisClass.VesselCluster[] vesselCluster)
        {
            if (_vesselCluster == null)
                return false;

            vesselCluster = _vesselCluster;
            return true;
        }
        #endregion (interface)

        #region Stochastics  -------------------------------------------------------------------------------------------------------

        protected bool CalculateMeanAreaValues()
        {
            if (_vesselInTissue == null)
                return false;
            Double[] vesselArea = new Double[_vesselInTissue.Length];
            try
            {
                for (Int32 V = 0; V < _vesselInTissue.Length; V++)
                    vesselArea[V] = _vesselInTissue[V]._area;
            }
            catch
            {
                return false;
            }

            try
            {
                _normalizedVesselGeometry.meanArea = _stochastics.MeanValue(vesselArea);
                _normalizedVesselGeometry.stdArea = _stochastics.StandardDeviation(vesselArea, _normalizedVesselGeometry.meanArea);
            }
            catch
            {
                return false;
            }


            return true;
        }
        protected bool CalculateMeanBoundingBox()
        {
            if (_vesselInTissue == null)
                return false;
            Double dMeanWidth = 0.0;
            Double dMeanHeight = 0.0;
            Double dStdWidth = 0.0;
            Double dStdHeight = 0.0;

            Double[] vesselWidth = new Double[_vesselInTissue.Length];
            Double[] vesselHeight = new Double[_vesselInTissue.Length];

            for (Int32 V = 0; V < _vesselInTissue.Length; V++)
            {
                vesselWidth[V] = _vesselInTissue[V]._boundingBox.Width;
                vesselHeight[V] = _vesselInTissue[V]._boundingBox.Height;
            }

            try
            {
                dMeanWidth = _stochastics.MeanValue(vesselWidth);
                dStdWidth = _stochastics.StandardDeviation(vesselWidth, dMeanWidth);
                dMeanHeight = _stochastics.MeanValue(vesselHeight);
                dStdHeight = _stochastics.StandardDeviation(vesselHeight, dMeanHeight);
            }
            catch
            {
                return false;
            }
            _normalizedVesselGeometry.meanWidth = dMeanWidth;
            _normalizedVesselGeometry.stdWidth = dStdWidth;
            _normalizedVesselGeometry.meanHeight = dMeanHeight;
            _normalizedVesselGeometry.stdHeight = dStdHeight;
            return true;
        }

        #endregion (stochastics)

        #region Cluster vessels ----------------------------------------------------------------------------------------------------
        protected void AddCluster(ref Int32 addedVc, Int32 vId)
        {
            VesselCluster[] tmp = null;
            Int32 elements = 0;

            if (_vesselCluster != null)
                elements = _vesselCluster.Length;

            tmp = new VesselCluster[elements + 1];

            for (Int32 Vc = 0; Vc < elements; Vc++)
                tmp[Vc] = _vesselCluster[Vc];

            _vesselInTissue[vId]._belongsToCluster = true;
            tmp[elements].id = elements;
            tmp[elements].vessels = new VesselClass[1];
            tmp[elements].vessels[0] = _vesselInTissue[vId];
            _vesselCluster = tmp;
            addedVc = _vesselCluster.Length - 1;
        }

        protected void AddVesselToCluster(Int32 vC, Int32 vId)
        {
            VesselClass[] tmp = null;
            Int32 elements = 0;

            if (_vesselCluster[vC].vessels != null)
                elements = _vesselCluster[vC].vessels.Length;

            tmp = new VesselClass[elements + 1];

            for (Int32 V = 0; V < elements; V++)
                tmp[V] = _vesselCluster[vC].vessels[V];

            tmp[elements] = _vesselInTissue[vId];
            _vesselCluster[vC].vessels = tmp;
        }

        protected bool AllVesselsSortedToClusters(ref Int32 Vc)
        {
            for (Int32 V = 0; V < _vesselInTissue.Length; V++)
            {
                if (_vesselInTissue[V]._belongsToCluster == false)
                {
                    AddCluster(ref Vc, V);
                    return false;
                }
            }
            return true;
        }

        protected bool DoClusterAnalysis()
        {
            bool allVesselsBelongToClusters = false;
            bool newVesselAdded = false;
            bool somethingAdded = false;
            Int32 Vc = 0;
            //_parentForm.labOutPut.Text = "Cluster analysis: recoursive structure scan...";
            Console.WriteLine("Cluster analysis: recoursive structure scan...");
            System.Windows.Forms.Application.DoEvents();
            Int32 counter = 1;

            do
            {
                //_parentForm.labOutPut.Text = "Cluster analysis: recoursive structure scan... sorting (" + counter.ToString() + ")";
                Console.WriteLine("Cluster analysis: recoursive structure scan... sorting (" + counter.ToString() + ")");
                System.Windows.Forms.Application.DoEvents();
                allVesselsBelongToClusters = AllVesselsSortedToClusters(ref Vc);
                do
                {
                    newVesselAdded = false;
                    for (Int32 V = 0; V < _vesselInTissue.Length; V++) // Try to sort all vessels to the current cluster
                    {
                        somethingAdded = false;

                        if (_vesselInTissue[V]._belongsToCluster == false)
                            somethingAdded = SortVesselsToCluster(Vc, V);

                        if (newVesselAdded == false && somethingAdded == true)
                            newVesselAdded = true;

                        counter++;
                    }
                } while (newVesselAdded == true); // To receive all vessels, even those coming late to neighborhood
            } while (allVesselsBelongToClusters == false);

            if (_vesselCluster == null)
                return false;

            if (_vesselCluster.Length == 0)
                return false;

            return true;
        }

        protected void InitClusterAnalysis()
        {
            if (_vesselCluster != null)
            {
                for (Int32 Vc = 0; Vc < _vesselCluster.Length; Vc++)
                    _vesselCluster[Vc].vessels = null;

                _vesselCluster = null;
            }
            for (Int32 V = 0; V < _vesselInTissue.Length; V++)  // Eliminate all cluster flags
                _vesselInTissue[V]._belongsToCluster = false;
        }

        protected bool SortVesselsToCluster(Int32 vC, Int32 vId)
        {
            if (_vesselCluster == null)
                return false;

            if (VesselOfNearNeighborhood(vC, vId) == true)
            {
                _vesselInTissue[vId]._belongsToCluster = true;  // Eliminate that vessel from analysis 
                AddVesselToCluster(vC, vId);
                return true;
            }

            return false;
        }

        protected bool VesselOfNearNeighborhood(Int32 vC, Int32 vId)
        {
            Int32 centerVesselX = _vesselInTissue[vId]._gravCenter.X;
            Int32 centerVesselY = _vesselInTissue[vId]._gravCenter.Y;
            Int32 widthOfCenterVessel = _vesselInTissue[vId]._boundingBox.Width;
            Int32 heightOfCenterVessel = _vesselInTissue[vId]._boundingBox.Height;

            Int32 centerVesselOfClusterX;
            Int32 centerVesselOfClusterY;
            Int32 widthVesselOfCluster;
            Int32 heightVesselOfCluster;

            for (Int32 V = 0; V < _vesselCluster[vC].vessels.Length; V++)  // Investigate all vessels clustered, so far.
            {
                centerVesselOfClusterX = _vesselCluster[vC].vessels[V]._gravCenter.X;
                centerVesselOfClusterY = _vesselCluster[vC].vessels[V]._gravCenter.Y;
                widthVesselOfCluster = _vesselCluster[vC].vessels[V]._boundingBox.Width;
                heightVesselOfCluster = _vesselCluster[vC].vessels[V]._boundingBox.Height;

                if (VesselsIntersect(centerVesselX, centerVesselY, widthOfCenterVessel, heightOfCenterVessel,
                                    centerVesselOfClusterX, centerVesselOfClusterY, widthVesselOfCluster, heightVesselOfCluster) == true)
                    return true;
            }
            return false;
        }

        protected bool VesselsIntersect(Int32 centerVesselX, Int32 centerVesselY, Int32 widthOfCenterVessel, Int32 heightOfCenterVessel,
                                        Int32 centerClusterVesselX, Int32 centerClusterVesselY, Int32 widthVesselOfCluster, Int32 heightVesselOfCluster)
        {
            Int32 widthToleranceCenterVessel = (Int32)((widthOfCenterVessel / 2.0) * _dHorizontalToleranceMultiplicator);
            Int32 heightToleranceCenterVessel = (Int32)((heightOfCenterVessel / 2.0) * _dVerticalToleranceMultiplicator);
            Int32 widthToleranceClusterVessel = (Int32)((widthVesselOfCluster / 2.0) * _dHorizontalToleranceMultiplicator);
            Int32 heightToleranceClusterVessel = (Int32)((heightVesselOfCluster / 2.0) * _dVerticalToleranceMultiplicator);

            //--1. Eliminate identicals (the very first vessel of a cluster is always stored.
            if ((centerVesselX == centerClusterVesselX) &&
               (centerVesselX == centerClusterVesselX) &&
               (centerVesselY == centerClusterVesselY) &&
               (centerVesselY == centerClusterVesselY))
                return false;

            Int32 Xs1 = centerVesselX - widthToleranceCenterVessel;
            Int32 Ys1 = centerVesselY - heightToleranceCenterVessel;
            Int32 Xs2 = centerVesselX + widthToleranceCenterVessel;
            Int32 Ys2 = centerVesselY + heightToleranceCenterVessel;

            Int32 Xt1 = centerClusterVesselX - widthToleranceClusterVessel;
            Int32 Yt1 = centerClusterVesselY - heightToleranceClusterVessel;
            Int32 Xt2 = centerClusterVesselX + widthToleranceClusterVessel;
            Int32 Yt2 = centerClusterVesselY + heightToleranceClusterVessel;
            if ((Xt2 >= Xs1 && Xt1 <= Xs2) && (Yt2 >= Ys1 && Yt1 <= Ys2))
                return true;


            return false;

            /*
            //-- 2. Search for neighbors
            if((centerVesselX + widthToleranceCenterVessel) >= (centerClusterVesselX - widthToleranceClusterVessel) &&
               (centerVesselX - widthToleranceCenterVessel) <= (centerClusterVesselX + widthToleranceClusterVessel) &&
               (centerVesselY + heightToleranceCenterVessel) >= (centerClusterVesselY - heightToleranceClusterVessel) &&
               (centerVesselY - heightToleranceCenterVessel) <= (centerClusterVesselY + heightToleranceClusterVessel))
              return true;

            return false;     */
        }
        #endregion (cluster vessels)

        #region Supercluster analysis ----------------------------------------------------------------------------------------------
        public bool GetOrbital(Int32 vC, Int32 v, ref Double orbitalWidth, ref Double orbitalHeight)
        {
            if (_vesselCluster == null)
                return false;
            if (vC >= _vesselCluster.Length)
                return false;
            if (_vesselCluster[vC].vessels == null)
                return false;
            if (v >= _vesselCluster[vC].vessels.Length)
                return false;
            orbitalWidth = _vesselCluster[vC].vessels[v]._boundingBox.Width * _dHorizontalToleranceMultiplicator;
            orbitalHeight = _vesselCluster[vC].vessels[v]._boundingBox.Height * _dVerticalToleranceMultiplicator;
            return true;
        }
        protected bool GetOrbitalOfTissueVessel(Int32 v, ref Double orbitalWidth, ref Double orbitalHeight)
        {
            if (_vesselInTissue == null)
                return false;
            if (v >= _vesselInTissue.Length)
                return false;
            orbitalWidth = _vesselInTissue[v]._boundingBox.Width * _dHorizontalToleranceMultiplicator;
            orbitalHeight = _vesselInTissue[v]._boundingBox.Height * _dVerticalToleranceMultiplicator;
            return true;
        }
        protected bool SuperClusterAnalysis()
        {
            if (_vesselCluster == null)
                return false;
            if (_vesselCluster.Length <= 0)
                return false;

            SortClusters();        // All vessels in all clusters are sorted in y++ direction
            bool superClusterGenerated = false;
            Int32 vC = 0;
            Int32 c = 1;
            //    Int32 testCounter=0;  // Testzwecke
            Int32 counter = 1;
            do
            {
                /// superClusterGenerated=false;       
                superClusterGenerated = true; // Testzwecke       
                do
                {
                    if (FarNeighborhoodOfClusters(vC, ref c) == true)
                    {
                        MergeClusters(vC, c);
                        SortClusters(); // After a new supercluster is generated, its vessels have to be resorted (the new cluster was "above", so that the topmost one will be the one of the new incoming cluster)
                        vC = 0;
                        c = 1;
                        superClusterGenerated = true;
                        //_parentForm.labOutPut.Text = "Joining clouds to super clusters...recoursive meshing: " + counter.ToString();
                        Console.WriteLine("Joining clouds to super clusters...recoursive meshing: " + counter.ToString());
                        System.Windows.Forms.Application.DoEvents();
                        counter++;
                        //   testCounter++;
                        //   if(testCounter >=27)
                        //     break; // Testzwecke
                    }
                    else
                    {
                        vC++;
                        if (vC >= _vesselCluster.Length)
                            break;
                    }
                } while (vC < _vesselCluster.Length);
                if (vC >= _vesselCluster.Length)
                    break;
            } while (superClusterGenerated == false);  // Testzwecke: nur ein Merge

            return true;
        }
        protected void SortClusters()
        {
            Int32 yo = 0;
            Int32 y1 = 0;
            for (Int32 vC = 0; vC < _vesselCluster.Length; vC++)
            {
                Array.Sort(_vesselCluster[vC].vessels, (x, y) => x._gravCenter.Y.CompareTo(y._gravCenter.Y));
                yo = _vesselCluster[vC].vessels[0]._gravCenter.Y;
                y1 = _vesselCluster[vC].vessels[_vesselCluster[vC].vessels.Length - 1]._gravCenter.Y;
            }
        }
        protected bool MergeClusters(Int32 C, Int32 c)
        {
            VesselCluster[] tmp = null;
            Int32 elements = 0;

            if (_vesselCluster != null)
                elements = _vesselCluster.Length;

            tmp = new VesselCluster[elements - 1];     // Merging: one cluster gets dispensed in anotherone

            //--1. Merge vessels to supercluster
            VesselClass[] tmpVes = null;
            Int32 v = 0;
            tmpVes = new VesselClass[_vesselCluster[C].vessels.Length + _vesselCluster[c].vessels.Length];
            for (Int32 vs = 0; vs < _vesselCluster[C].vessels.Length; vs++) // these clusters remain untouched
                tmpVes[v++] = _vesselCluster[C].vessels[vs];
            for (Int32 vs = 0; vs < _vesselCluster[c].vessels.Length; vs++) // these clusters remain untouched
                tmpVes[v++] = _vesselCluster[c].vessels[vs];

            _vesselCluster[C].vessels = tmpVes;

            //--2. reduce the vesselcluster-vector and eliminate mini-cluster
            Int32 vc = 0;
            for (Int32 vC = 0; vC < c; vC++)          // these clusters remain untouched
                tmp[vc++] = _vesselCluster[vC];
            // eliminate [c]
            for (Int32 vC = c + 1; vC < elements; vC++) // these clusters remain untouched
                tmp[vc++] = _vesselCluster[vC];

            _vesselCluster = tmp;

            return true;
        }
        protected bool FarNeighborhoodOfClusters(Int32 superCluster, ref Int32 testCluster)
        {
            /*
               Algorithm:

                         1. Define targeting spot, which is on top in the center of the tip

                         2. Calculate a target spot field and test, whether there is a cluster touched through that field

                         3. Decision and loop                   
                         3.1. If a neighbor cluster is found:
                              Check if that neighbor cluster owns vessels which are located on y coordinates below 
                              the search vessel's y coordinates DO NOT TAKE THAT NEIGHBOR AND THAT CLUSTER THEN!


            */


            Point testPoint = new Point(0, 0);
            Int32 testClusterVesselHit = -1;

            spotField[] testSpots = new spotField[36 + 1];   // Structural element for tesing.

            if (_vesselCluster[superCluster].saveCluster == false)   // unsafe clusters are  not investigated
                return false;

            if (GetTestPositionInCluster(superCluster, ref testPoint))
            {
                if (GenerateTestField(ref testSpots, superCluster, testPoint) == false)     // Generate test fields with given spot geometry (in v)
                    return false;                                      // Do not use a mid spot witdh

                _vesselCluster[superCluster].testSpots = testSpots;
                for (Int32 testSpot = 1; testSpot <= 36; testSpot++)                    // Test the spots of given structured element
                {
                    if (NeighborPresent(superCluster, testSpots[testSpot], ref testCluster, ref testClusterVesselHit) == true)    // neighbor cluster detected
                    {
                        return true;
                        //return false; // Testzwecke
                    }
                }
            }
            return false;
        }

        protected bool GetTestPositionInCluster(Int32 superCluster, ref Point testPoint)
        {
            Int32 candidates = 0;
            if (_vesselCluster[superCluster].vessels.Length > 5)           // Use the first 5 topmost vessels for tip geometry analysis
                candidates = 5;
            else
                candidates = _vesselCluster[superCluster].vessels.Length;   // Weak field, take all.

            Int32 xMin = Int32.MaxValue;
            Int32 yMin = Int32.MaxValue;
            Int32 xMax = Int32.MinValue;
            Int32 yMax = Int32.MinValue;

            for (Int32 v = 0; v < candidates; v++)
            {
                xMin = (xMin > _vesselCluster[superCluster].vessels[v]._gravCenter.X) ? _vesselCluster[superCluster].vessels[v]._gravCenter.X : xMin;
                yMin = (yMin > _vesselCluster[superCluster].vessels[v]._boundingBox.Y) ? _vesselCluster[superCluster].vessels[v]._boundingBox.Y : yMin;
                xMax = (xMax < _vesselCluster[superCluster].vessels[v]._gravCenter.X) ? _vesselCluster[superCluster].vessels[v]._gravCenter.X : xMax;
                yMax = (yMax < (_vesselCluster[superCluster].vessels[v]._boundingBox.Y + _vesselCluster[superCluster].vessels[v]._boundingBox.Height)) ? (_vesselCluster[superCluster].vessels[v]._boundingBox.Y + _vesselCluster[superCluster].vessels[v]._boundingBox.Height) : yMax;
            }

            testPoint.X = xMin + ((xMax - xMin) / 2);
            testPoint.Y = yMin;

            return true;
        }

        protected bool GenerateTestField(ref spotField[] testSpots, Int32 superCluster, Point testPoint)
        {
            //  Int32 x= _vesselCluster[superCluster].vessels[v]._gravCenter.X;
            //  Int32 y= _vesselCluster[superCluster].vessels[v]._gravCenter.Y;
            //  Int32 w= _vesselCluster[superCluster].vessels[v]._boundingBox.Width;
            //  Int32 h= _vesselCluster[superCluster].vessels[v]._boundingBox.Height;
            //  Int32 whalf= w/2;
            //  Int32 hhalf= h/2;
            Int32 x = testPoint.X;
            Int32 y = testPoint.Y;
            Int32 w = Convert.ToInt32(_normalizedVesselGeometry.meanWidth);
            Int32 h = Convert.ToInt32(_normalizedVesselGeometry.meanHeight);
            Int32 whalf = w / 2;
            Int32 hhalf = h / 2;


            DefineSpotField(ref testSpots[0], x, y, w, h, whalf, hhalf);  // Center spot

            #region First mid range field row (1, 2, 3, 4, 5, 6)

            DefineSpotField(ref testSpots[1], x, y - h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[2], x, y - 2 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[3], x, y - 3 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[4], x, y - 4 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[5], x, y - 5 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[6], x, y - 6 * h, w, h, whalf, hhalf);

            #endregion first mid range field row (1, 2, 3, 4, 5, 6)

            #region Second mid range field row (7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18)
            //--1. Left row
            DefineSpotField(ref testSpots[8], x - w, y - h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[10], x - w, y - 2 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[12], x - w, y - 3 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[14], x - w, y - 4 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[16], x - w, y - 5 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[18], x - w, y - 6 * h, w, h, whalf, hhalf);

            //--2. Right row
            DefineSpotField(ref testSpots[7], x + w, y - h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[9], x + w, y - 2 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[11], x + w, y - 3 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[13], x + w, y - 4 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[15], x + w, y - 5 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[17], x + w, y - 6 * h, w, h, whalf, hhalf);

            #endregion Second mid range field row (7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18)

            #region Third mid range field row (19, 20, 21, 22, 23, 24. 25, 26, 27, 28)
            //--1. Left row     
            DefineSpotField(ref testSpots[20], x - 2 * w, y - 2 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[22], x - 2 * w, y - 3 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[24], x - 2 * w, y - 4 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[26], x - 2 * w, y - 5 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[28], x - 2 * w, y - 6 * h, w, h, whalf, hhalf);

            //--2. Right row     
            DefineSpotField(ref testSpots[19], x + 2 * w, y - 2 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[21], x + 2 * w, y - 3 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[23], x + 2 * w, y - 4 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[25], x + 2 * w, y - 5 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[27], x + 2 * w, y - 6 * h, w, h, whalf, hhalf);

            #endregion third mid range field row (19, 20, 21, 22, 23, 24. 25, 26, 27, 28)

            #region Fourth mid range field row (29, 30, 31, 32)
            //-- 1. Left row
            DefineSpotField(ref testSpots[30], x - 3 * w, y - 3 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[32], x - 3 * w, y - 4 * h, w, h, whalf, hhalf);
            //-- 2. Right row
            DefineSpotField(ref testSpots[29], x + 3 * w, y - 3 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[31], x + 3 * w, y - 4 * h, w, h, whalf, hhalf);

            #endregion fourth mid range field row (29, 30, 31, 32)

            #region Side range field rows (33, 34, 35, 36)
            //-- 1. Left side
            DefineSpotField(ref testSpots[34], x - 4 * w, y - 4 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[36], x - 5 * w, y - 4 * h, w, h, whalf, hhalf);
            //-- 2. Right side
            DefineSpotField(ref testSpots[33], x + 4 * w, y - 4 * h, w, h, whalf, hhalf);
            DefineSpotField(ref testSpots[35], x + 5 * w, y - 4 * h, w, h, whalf, hhalf);

            #endregion side range field rows (33, 34, 35, 36)


            return true;
        }
        protected void DefineSpotField(ref spotField testSpot, Int32 x, Int32 y, Int32 w, Int32 h, Int32 whalf, Int32 hhalf)
        {
            testSpot.Xc = x;
            testSpot.Yc = y;
            testSpot.Xo = x - whalf;
            testSpot.Yo = y - hhalf;
            testSpot.X1 = x + whalf;
            testSpot.Y1 = y + hhalf;
        }
        protected bool GetTopmostRightVesselInCluster(Int32 sC, ref Int32 v, Int32 maxHeightsBelowTopsite, ref Int32 yStripeBefore, Int32 lastVesselFound)
        {
            /*
                All vessels are sorted in y++ direction. The three top vessels are located at [0] .. [2]
                We need three vessels, which are located at the right side of the cluster
             */
            Int32 uniHeight = 0;

            try
            {
                uniHeight = Convert.ToInt32(_normalizedVesselGeometry.meanHeight * maxHeightsBelowTopsite);
            }
            catch
            {
                return false;
            }

            if (yStripeBefore == -1)   // Indicator for not being set, yet.
                yStripeBefore = _vesselCluster[sC].vessels[0]._boundingBox.Y;
            Int32 yCutOff = yStripeBefore + uniHeight;  // That is the borderline defining the vessels which are allowed to be investigated

            //--1. Define pack of vessels which come into focus of investigation. 
            Int32 startThresholdVessel = -1;
            Int32 stopThresholdVessel = 0;        // Vessel which defines cutoff

            Int32 vesselBottom = 0;

            for (Int32 T = 0; T < _vesselCluster[sC].vessels.Length; T++)
            {
                vesselBottom = _vesselCluster[sC].vessels[T]._boundingBox.Y;
                if (vesselBottom >= yStripeBefore)       // Get the toptip, if the one.
                {
                    if (startThresholdVessel == -1)
                        startThresholdVessel = T;
                    else
                        stopThresholdVessel = T;

                    if (vesselBottom > yCutOff)
                        break;
                }
            }

            //--2. Take the one within this pack, which gravity center is most to the right             
            Int32 maxX = Int32.MinValue;
            v = 0;
            if (startThresholdVessel == -1)   // nothing found
                return false;
            for (Int32 T = startThresholdVessel; T < stopThresholdVessel; T++)
            {
                if (maxX < _vesselCluster[sC].vessels[T]._gravCenter.X && T != lastVesselFound)
                {
                    maxX = _vesselCluster[sC].vessels[T]._gravCenter.X;
                    v = T;
                }
            }

            yStripeBefore = yCutOff;
            return true;
        }
        protected bool NeighborPresent(Int32 superCluster, spotField testSpot, ref Int32 testClusterHit, ref Int32 vesselHitInTestCluster)
        {
            for (Int32 cV = 0; cV < _vesselCluster.Length; cV++)   // Test all Clusters
            {
                if (cV == superCluster)                            // No selftest
                    continue;

                for (Int32 V = 0; V < _vesselCluster[cV].vessels.Length; V++)   // All vessels in cluster
                {
                    if (VesselHit(_vesselCluster[cV].vessels[V], testSpot) == true)
                    {
                        if (ValidClusterHit(superCluster, cV) == true)            // Test, if any of the vessels are in parallel to the left or right of this cluster
                        {
                            testClusterHit = cV;             // That cluster may be merged to the supercluster
                            vesselHitInTestCluster = V;      // That vessel was the one which interfered
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected bool VesselHit(VesselClass vessel, spotField testSpot)
        {
            Int32 orbitalX = Convert.ToInt32((vessel._boundingBox.Width / 2.0)  /* * _dHorizontalToleranceMultiplicator  */);
            Int32 orbitalY = Convert.ToInt32((vessel._boundingBox.Height / 2.0) /*   * _dHorizontalToleranceMultiplicator   */);

            // Two rectangles intersect if and only if (Xt2 >= Xs1 && Xt1 <= Xs2) && (Yt2 >= Ys1 && Yt1 <= Ys2)
            Int32 Xs1 = testSpot.Xo;
            Int32 Ys1 = testSpot.Yo;
            Int32 Xs2 = testSpot.X1;
            Int32 Ys2 = testSpot.Y1;

            Int32 Xt1 = vessel._boundingBox.X - orbitalX;
            Int32 Yt1 = vessel._boundingBox.Y - orbitalY;
            Int32 Xt2 = vessel._boundingBox.X + vessel._boundingBox.Width + orbitalX;
            Int32 Yt2 = vessel._boundingBox.Y + vessel._boundingBox.Height + orbitalY;



            if ((Xt2 >= Xs1 && Xt1 <= Xs2) && (Yt2 >= Ys1 && Yt1 <= Ys2))
                return true;


            return false;
        }

        protected bool ValidClusterHit(Int32 superCluster, Int32 testCluster)
        {
            if (superCluster == testCluster)
                return false;
            for (Int32 sCv = 0; sCv < _vesselCluster[superCluster].vessels.Length; sCv++)
            {
                for (Int32 sTv = 0; sTv < _vesselCluster[testCluster].vessels.Length; sTv++)
                {
                    if (HorizontallyInterferingVessels(_vesselCluster[superCluster].vessels[sCv], _vesselCluster[testCluster].vessels[sTv]) == true)   // These are parallel!
                        return false;
                }
            }
            return true;
        }
        protected bool HorizontallyInterferingVessels(VesselClass superClusterVessel, VesselClass testClusterVessel)
        {
            Int32 Yscv = superClusterVessel._gravCenter.Y;
            // Int32 Hscv= superClusterVessel._boundingBox.Height/2;
            Int32 Ytcv = testClusterVessel._gravCenter.Y;
            // Int32 Htcv= testClusterVessel._boundingBox.Height/2;

            if (Ytcv > Yscv)   // Attention: technical y coordinates. y++ is "below". if any of the test vessels are below the ones of the super cluster vessel, it is wrong.
                return true;

            return false;
        }
        #endregion (supercluster analysis)
    }
}
