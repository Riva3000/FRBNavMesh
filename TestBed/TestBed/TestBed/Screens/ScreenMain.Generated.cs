#if ANDROID || IOS || DESKTOP_GL
#define REQUIRES_PRIMARY_THREAD_LOADING
#endif
using Color = Microsoft.Xna.Framework.Color;
using FlatRedBall;
using FlatRedBall.Screens;
using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall.Math.Geometry;
namespace TestBed.Screens
{
    public partial class ScreenMain : FlatRedBall.Screens.Screen
    {
        #if DEBUG
        static bool HasBeenLoadedWithGlobalContentManager = false;
        #endif
        
        private FlatRedBall.Math.Geometry.Polygon mCenterH;
        public FlatRedBall.Math.Geometry.Polygon CenterH
        {
            get
            {
                return mCenterH;
            }
            private set
            {
                mCenterH = value;
            }
        }
        private FlatRedBall.Math.Geometry.Polygon mCenterV;
        public FlatRedBall.Math.Geometry.Polygon CenterV
        {
            get
            {
                return mCenterV;
            }
            private set
            {
                mCenterV = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRect1Main;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle Rect1Main
        {
            get
            {
                return mRect1Main;
            }
            private set
            {
                mRect1Main = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRect2InnerTouching;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle Rect2InnerTouching
        {
            get
            {
                return mRect2InnerTouching;
            }
            private set
            {
                mRect2InnerTouching = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRect3InnerTouching;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle Rect3InnerTouching
        {
            get
            {
                return mRect3InnerTouching;
            }
            private set
            {
                mRect3InnerTouching = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRect4InnerTouching;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle Rect4InnerTouching
        {
            get
            {
                return mRect4InnerTouching;
            }
            private set
            {
                mRect4InnerTouching = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRect5InnerTouching;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle Rect5InnerTouching
        {
            get
            {
                return mRect5InnerTouching;
            }
            private set
            {
                mRect5InnerTouching = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRect6OuterTouching;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle Rect6OuterTouching
        {
            get
            {
                return mRect6OuterTouching;
            }
            private set
            {
                mRect6OuterTouching = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRect7OuterTouching;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle Rect7OuterTouching
        {
            get
            {
                return mRect7OuterTouching;
            }
            private set
            {
                mRect7OuterTouching = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRect8OuterNotTouching;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle Rect8OuterNotTouching
        {
            get
            {
                return mRect8OuterNotTouching;
            }
            private set
            {
                mRect8OuterNotTouching = value;
            }
        }
        public ScreenMain () 
        	: base ("ScreenMain")
        {
        }
        public override void Initialize (bool addToManagers) 
        {
            LoadStaticContent(ContentManagerName);
            mCenterH = new FlatRedBall.Math.Geometry.Polygon();
            mCenterH.Name = "mCenterH";
            mCenterV = new FlatRedBall.Math.Geometry.Polygon();
            mCenterV.Name = "mCenterV";
            mRect1Main = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRect1Main.Name = "mRect1Main";
            mRect2InnerTouching = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRect2InnerTouching.Name = "mRect2InnerTouching";
            mRect3InnerTouching = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRect3InnerTouching.Name = "mRect3InnerTouching";
            mRect4InnerTouching = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRect4InnerTouching.Name = "mRect4InnerTouching";
            mRect5InnerTouching = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRect5InnerTouching.Name = "mRect5InnerTouching";
            mRect6OuterTouching = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRect6OuterTouching.Name = "mRect6OuterTouching";
            mRect7OuterTouching = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRect7OuterTouching.Name = "mRect7OuterTouching";
            mRect8OuterNotTouching = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRect8OuterNotTouching.Name = "mRect8OuterNotTouching";
            
            
            PostInitialize();
            base.Initialize(addToManagers);
            if (addToManagers)
            {
                AddToManagers();
            }
        }
        public override void AddToManagers () 
        {
            FlatRedBall.Math.Geometry.ShapeManager.AddPolygon(mCenterH);
            FlatRedBall.Math.Geometry.ShapeManager.AddPolygon(mCenterV);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRect1Main);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRect2InnerTouching);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRect3InnerTouching);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRect4InnerTouching);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRect5InnerTouching);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRect6OuterTouching);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRect7OuterTouching);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRect8OuterNotTouching);
            base.AddToManagers();
            AddToManagersBottomUp();
            CustomInitialize();
        }
        public override void Activity (bool firstTimeCalled) 
        {
            if (!IsPaused)
            {
                
            }
            else
            {
            }
            base.Activity(firstTimeCalled);
            if (!IsActivityFinished)
            {
                CustomActivity(firstTimeCalled);
            }
        }
        public override void Destroy () 
        {
            base.Destroy();
            
            if (CenterH != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(CenterH);
            }
            if (CenterV != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(CenterV);
            }
            if (Rect1Main != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(Rect1Main);
            }
            if (Rect2InnerTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(Rect2InnerTouching);
            }
            if (Rect3InnerTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(Rect3InnerTouching);
            }
            if (Rect4InnerTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(Rect4InnerTouching);
            }
            if (Rect5InnerTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(Rect5InnerTouching);
            }
            if (Rect6OuterTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(Rect6OuterTouching);
            }
            if (Rect7OuterTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(Rect7OuterTouching);
            }
            if (Rect8OuterNotTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(Rect8OuterNotTouching);
            }
            FlatRedBall.Math.Collision.CollisionManager.Self.Relationships.Clear();
            CustomDestroy();
        }
        public virtual void PostInitialize () 
        {
            bool oldShapeManagerSuppressAdd = FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = true;
            FlatRedBall.Math.Geometry.Point[] CenterHPoints = new FlatRedBall.Math.Geometry.Point[] {new FlatRedBall.Math.Geometry.Point(-25, 0), new FlatRedBall.Math.Geometry.Point(25, 0) };
            CenterH.Points = CenterHPoints;
            FlatRedBall.Math.Geometry.Point[] CenterVPoints = new FlatRedBall.Math.Geometry.Point[] {new FlatRedBall.Math.Geometry.Point(0, 25), new FlatRedBall.Math.Geometry.Point(0, -25) };
            CenterV.Points = CenterVPoints;
            Rect1Main.Width = 100f;
            Rect1Main.Height = 100f;
            Rect1Main.Color = Microsoft.Xna.Framework.Color.SkyBlue;
            if (Rect2InnerTouching.Parent == null)
            {
                Rect2InnerTouching.X = 50f;
            }
            else
            {
                Rect2InnerTouching.RelativeX = 50f;
            }
            if (Rect2InnerTouching.Parent == null)
            {
                Rect2InnerTouching.Y = -100f;
            }
            else
            {
                Rect2InnerTouching.RelativeY = -100f;
            }
            Rect2InnerTouching.Width = 100f;
            Rect2InnerTouching.Height = 100f;
            Rect2InnerTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            if (Rect3InnerTouching.Parent == null)
            {
                Rect3InnerTouching.X = -100f;
            }
            else
            {
                Rect3InnerTouching.RelativeX = -100f;
            }
            if (Rect3InnerTouching.Parent == null)
            {
                Rect3InnerTouching.Y = -20f;
            }
            else
            {
                Rect3InnerTouching.RelativeY = -20f;
            }
            Rect3InnerTouching.Width = 100f;
            Rect3InnerTouching.Height = 100f;
            Rect3InnerTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            if (Rect4InnerTouching.Parent == null)
            {
                Rect4InnerTouching.X = 60f;
            }
            else
            {
                Rect4InnerTouching.RelativeX = 60f;
            }
            if (Rect4InnerTouching.Parent == null)
            {
                Rect4InnerTouching.Y = 55f;
            }
            else
            {
                Rect4InnerTouching.RelativeY = 55f;
            }
            Rect4InnerTouching.Width = 20f;
            Rect4InnerTouching.Height = 30f;
            Rect4InnerTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            if (Rect5InnerTouching.Parent == null)
            {
                Rect5InnerTouching.X = -105f;
            }
            else
            {
                Rect5InnerTouching.RelativeX = -105f;
            }
            if (Rect5InnerTouching.Parent == null)
            {
                Rect5InnerTouching.Y = 90f;
            }
            else
            {
                Rect5InnerTouching.RelativeY = 90f;
            }
            Rect5InnerTouching.Width = 150f;
            Rect5InnerTouching.Height = 80f;
            Rect5InnerTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            if (Rect6OuterTouching.Parent == null)
            {
                Rect6OuterTouching.X = 70f;
            }
            else
            {
                Rect6OuterTouching.RelativeX = 70f;
            }
            if (Rect6OuterTouching.Parent == null)
            {
                Rect6OuterTouching.Y = 120f;
            }
            else
            {
                Rect6OuterTouching.RelativeY = 120f;
            }
            Rect6OuterTouching.Width = 200f;
            Rect6OuterTouching.Height = 100f;
            Rect6OuterTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            if (Rect7OuterTouching.Parent == null)
            {
                Rect7OuterTouching.X = -250f;
            }
            else
            {
                Rect7OuterTouching.RelativeX = -250f;
            }
            if (Rect7OuterTouching.Parent == null)
            {
                Rect7OuterTouching.Y = 24f;
            }
            else
            {
                Rect7OuterTouching.RelativeY = 24f;
            }
            Rect7OuterTouching.Width = 40f;
            Rect7OuterTouching.Height = 100f;
            Rect7OuterTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            if (Rect8OuterNotTouching.Parent == null)
            {
                Rect8OuterNotTouching.X = -100f;
            }
            else
            {
                Rect8OuterNotTouching.RelativeX = -100f;
            }
            if (Rect8OuterNotTouching.Parent == null)
            {
                Rect8OuterNotTouching.Y = -170f;
            }
            else
            {
                Rect8OuterNotTouching.RelativeY = -170f;
            }
            Rect8OuterNotTouching.Width = 100f;
            Rect8OuterNotTouching.Height = 200f;
            Rect8OuterNotTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = oldShapeManagerSuppressAdd;
        }
        public virtual void AddToManagersBottomUp () 
        {
            CameraSetup.ResetCamera(SpriteManager.Camera);
            AssignCustomVariables(false);
        }
        public virtual void RemoveFromManagers () 
        {
            if (CenterH != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(CenterH);
            }
            if (CenterV != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(CenterV);
            }
            if (Rect1Main != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(Rect1Main);
            }
            if (Rect2InnerTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(Rect2InnerTouching);
            }
            if (Rect3InnerTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(Rect3InnerTouching);
            }
            if (Rect4InnerTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(Rect4InnerTouching);
            }
            if (Rect5InnerTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(Rect5InnerTouching);
            }
            if (Rect6OuterTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(Rect6OuterTouching);
            }
            if (Rect7OuterTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(Rect7OuterTouching);
            }
            if (Rect8OuterNotTouching != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(Rect8OuterNotTouching);
            }
        }
        public virtual void AssignCustomVariables (bool callOnContainedElements) 
        {
            if (callOnContainedElements)
            {
            }
            Rect1Main.Width = 100f;
            Rect1Main.Height = 100f;
            Rect1Main.Color = Microsoft.Xna.Framework.Color.SkyBlue;
            if (Rect2InnerTouching.Parent == null)
            {
                Rect2InnerTouching.X = 50f;
            }
            else
            {
                Rect2InnerTouching.RelativeX = 50f;
            }
            if (Rect2InnerTouching.Parent == null)
            {
                Rect2InnerTouching.Y = -100f;
            }
            else
            {
                Rect2InnerTouching.RelativeY = -100f;
            }
            Rect2InnerTouching.Width = 100f;
            Rect2InnerTouching.Height = 100f;
            Rect2InnerTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            if (Rect3InnerTouching.Parent == null)
            {
                Rect3InnerTouching.X = -100f;
            }
            else
            {
                Rect3InnerTouching.RelativeX = -100f;
            }
            if (Rect3InnerTouching.Parent == null)
            {
                Rect3InnerTouching.Y = -20f;
            }
            else
            {
                Rect3InnerTouching.RelativeY = -20f;
            }
            Rect3InnerTouching.Width = 100f;
            Rect3InnerTouching.Height = 100f;
            Rect3InnerTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            if (Rect4InnerTouching.Parent == null)
            {
                Rect4InnerTouching.X = 60f;
            }
            else
            {
                Rect4InnerTouching.RelativeX = 60f;
            }
            if (Rect4InnerTouching.Parent == null)
            {
                Rect4InnerTouching.Y = 55f;
            }
            else
            {
                Rect4InnerTouching.RelativeY = 55f;
            }
            Rect4InnerTouching.Width = 20f;
            Rect4InnerTouching.Height = 30f;
            Rect4InnerTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            if (Rect5InnerTouching.Parent == null)
            {
                Rect5InnerTouching.X = -105f;
            }
            else
            {
                Rect5InnerTouching.RelativeX = -105f;
            }
            if (Rect5InnerTouching.Parent == null)
            {
                Rect5InnerTouching.Y = 90f;
            }
            else
            {
                Rect5InnerTouching.RelativeY = 90f;
            }
            Rect5InnerTouching.Width = 150f;
            Rect5InnerTouching.Height = 80f;
            Rect5InnerTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            if (Rect6OuterTouching.Parent == null)
            {
                Rect6OuterTouching.X = 70f;
            }
            else
            {
                Rect6OuterTouching.RelativeX = 70f;
            }
            if (Rect6OuterTouching.Parent == null)
            {
                Rect6OuterTouching.Y = 120f;
            }
            else
            {
                Rect6OuterTouching.RelativeY = 120f;
            }
            Rect6OuterTouching.Width = 200f;
            Rect6OuterTouching.Height = 100f;
            Rect6OuterTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            if (Rect7OuterTouching.Parent == null)
            {
                Rect7OuterTouching.X = -250f;
            }
            else
            {
                Rect7OuterTouching.RelativeX = -250f;
            }
            if (Rect7OuterTouching.Parent == null)
            {
                Rect7OuterTouching.Y = 24f;
            }
            else
            {
                Rect7OuterTouching.RelativeY = 24f;
            }
            Rect7OuterTouching.Width = 40f;
            Rect7OuterTouching.Height = 100f;
            Rect7OuterTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
            if (Rect8OuterNotTouching.Parent == null)
            {
                Rect8OuterNotTouching.X = -100f;
            }
            else
            {
                Rect8OuterNotTouching.RelativeX = -100f;
            }
            if (Rect8OuterNotTouching.Parent == null)
            {
                Rect8OuterNotTouching.Y = -170f;
            }
            else
            {
                Rect8OuterNotTouching.RelativeY = -170f;
            }
            Rect8OuterNotTouching.Width = 100f;
            Rect8OuterNotTouching.Height = 200f;
            Rect8OuterNotTouching.Color = Microsoft.Xna.Framework.Color.Salmon;
        }
        public virtual void ConvertToManuallyUpdated () 
        {
        }
        public static void LoadStaticContent (string contentManagerName) 
        {
            if (string.IsNullOrEmpty(contentManagerName))
            {
                throw new System.ArgumentException("contentManagerName cannot be empty or null");
            }
            #if DEBUG
            if (contentManagerName == FlatRedBall.FlatRedBallServices.GlobalContentManager)
            {
                HasBeenLoadedWithGlobalContentManager = true;
            }
            else if (HasBeenLoadedWithGlobalContentManager)
            {
                throw new System.Exception("This type has been loaded with a Global content manager, then loaded with a non-global.  This can lead to a lot of bugs");
            }
            #endif
            CustomLoadStaticContent(contentManagerName);
        }
        [System.Obsolete("Use GetFile instead")]
        public static object GetStaticMember (string memberName) 
        {
            return null;
        }
        public static object GetFile (string memberName) 
        {
            return null;
        }
        object GetMember (string memberName) 
        {
            return null;
        }
    }
}
