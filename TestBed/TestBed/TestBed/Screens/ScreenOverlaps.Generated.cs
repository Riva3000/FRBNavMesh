#if ANDROID || IOS || DESKTOP_GL
#define REQUIRES_PRIMARY_THREAD_LOADING
#endif
using Color = Microsoft.Xna.Framework.Color;
using TestBed.Entities;
using FlatRedBall;
using FlatRedBall.Screens;
using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall.Math;
using FlatRedBall.Math.Geometry;
namespace TestBed.Screens
{
    public partial class ScreenOverlaps : FlatRedBall.Screens.Screen
    {
        #if DEBUG
        static bool HasBeenLoadedWithGlobalContentManager = false;
        #endif
        
        private FlatRedBall.Math.PositionedObjectList<FlatRedBall.Math.Geometry.AxisAlignedRectangle> RectsList;
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRectMain;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle RectMain
        {
            get
            {
                return mRectMain;
            }
            private set
            {
                mRectMain = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRectSide1;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle RectSide1
        {
            get
            {
                return mRectSide1;
            }
            private set
            {
                mRectSide1 = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRectSide2;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle RectSide2
        {
            get
            {
                return mRectSide2;
            }
            private set
            {
                mRectSide2 = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRectBelow;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle RectBelow
        {
            get
            {
                return mRectBelow;
            }
            private set
            {
                mRectBelow = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRectBelow2;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle RectBelow2
        {
            get
            {
                return mRectBelow2;
            }
            private set
            {
                mRectBelow2 = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRectTop;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle RectTop
        {
            get
            {
                return mRectTop;
            }
            private set
            {
                mRectTop = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRectTop2;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle RectTop2
        {
            get
            {
                return mRectTop2;
            }
            private set
            {
                mRectTop2 = value;
            }
        }
        private FlatRedBall.Math.PositionedObjectList<FlatRedBall.Math.Geometry.AxisAlignedRectangle> RectsListUnused;
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRectTopCenter;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle RectTopCenter
        {
            get
            {
                return mRectTopCenter;
            }
            private set
            {
                mRectTopCenter = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRectBelowCenter;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle RectBelowCenter
        {
            get
            {
                return mRectBelowCenter;
            }
            private set
            {
                mRectBelowCenter = value;
            }
        }
        private TestBed.Entities.Arrows ArrowsInstance;
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRectLeftCenter;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle RectLeftCenter
        {
            get
            {
                return mRectLeftCenter;
            }
            private set
            {
                mRectLeftCenter = value;
            }
        }
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mRectRightCenter;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle RectRightCenter
        {
            get
            {
                return mRectRightCenter;
            }
            private set
            {
                mRectRightCenter = value;
            }
        }
        public ScreenOverlaps () 
        	: base ("ScreenOverlaps")
        {
        }
        public override void Initialize (bool addToManagers) 
        {
            LoadStaticContent(ContentManagerName);
            RectsList = new FlatRedBall.Math.PositionedObjectList<FlatRedBall.Math.Geometry.AxisAlignedRectangle>();
            RectsList.Name = "RectsList";
            mRectMain = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRectMain.Name = "mRectMain";
            mRectSide1 = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRectSide1.Name = "mRectSide1";
            mRectSide2 = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRectSide2.Name = "mRectSide2";
            mRectBelow = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRectBelow.Name = "mRectBelow";
            mRectBelow2 = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRectBelow2.Name = "mRectBelow2";
            mRectTop = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRectTop.Name = "mRectTop";
            mRectTop2 = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRectTop2.Name = "mRectTop2";
            RectsListUnused = new FlatRedBall.Math.PositionedObjectList<FlatRedBall.Math.Geometry.AxisAlignedRectangle>();
            RectsListUnused.Name = "RectsListUnused";
            mRectTopCenter = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRectTopCenter.Name = "mRectTopCenter";
            mRectBelowCenter = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRectBelowCenter.Name = "mRectBelowCenter";
            ArrowsInstance = new TestBed.Entities.Arrows(ContentManagerName, false);
            ArrowsInstance.Name = "ArrowsInstance";
            mRectLeftCenter = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRectLeftCenter.Name = "mRectLeftCenter";
            mRectRightCenter = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mRectRightCenter.Name = "mRectRightCenter";
            
            
            PostInitialize();
            base.Initialize(addToManagers);
            if (addToManagers)
            {
                AddToManagers();
            }
        }
        public override void AddToManagers () 
        {
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRectMain);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRectSide1);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRectSide2);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRectBelow);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRectBelow2);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRectTop);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRectTop2);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRectTopCenter);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRectBelowCenter);
            ArrowsInstance.AddToManagers(mLayer);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRectLeftCenter);
            FlatRedBall.Math.Geometry.ShapeManager.AddAxisAlignedRectangle(mRectRightCenter);
            base.AddToManagers();
            AddToManagersBottomUp();
            CustomInitialize();
        }
        public override void Activity (bool firstTimeCalled) 
        {
            if (!IsPaused)
            {
                
                ArrowsInstance.Activity();
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
            
            RectsList.MakeOneWay();
            RectsListUnused.MakeOneWay();
            for (int i = RectsList.Count - 1; i > -1; i--)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(RectsList[i]);
            }
            for (int i = RectsListUnused.Count - 1; i > -1; i--)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(RectsListUnused[i]);
            }
            if (ArrowsInstance != null)
            {
                ArrowsInstance.Destroy();
                ArrowsInstance.Detach();
            }
            if (RectLeftCenter != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(RectLeftCenter);
            }
            if (RectRightCenter != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(RectRightCenter);
            }
            RectsList.MakeTwoWay();
            RectsListUnused.MakeTwoWay();
            FlatRedBall.Math.Collision.CollisionManager.Self.Relationships.Clear();
            CustomDestroy();
        }
        public virtual void PostInitialize () 
        {
            bool oldShapeManagerSuppressAdd = FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = true;
            RectsList.Add(RectMain);
            if (RectMain.Parent == null)
            {
                RectMain.X = 0f;
            }
            else
            {
                RectMain.RelativeX = 0f;
            }
            if (RectMain.Parent == null)
            {
                RectMain.Y = 0f;
            }
            else
            {
                RectMain.RelativeY = 0f;
            }
            RectMain.Width = 140f;
            RectMain.Height = 140f;
            RectsList.Add(RectSide1);
            if (RectSide1.Parent == null)
            {
                RectSide1.X = -120f;
            }
            else
            {
                RectSide1.RelativeX = -120f;
            }
            if (RectSide1.Parent == null)
            {
                RectSide1.Y = 0f;
            }
            else
            {
                RectSide1.RelativeY = 0f;
            }
            RectSide1.Width = 100f;
            RectSide1.Height = 100f;
            RectsList.Add(RectSide2);
            if (RectSide2.Parent == null)
            {
                RectSide2.X = 120f;
            }
            else
            {
                RectSide2.RelativeX = 120f;
            }
            if (RectSide2.Parent == null)
            {
                RectSide2.Y = 0f;
            }
            else
            {
                RectSide2.RelativeY = 0f;
            }
            RectSide2.Width = 100f;
            RectSide2.Height = 100f;
            RectsList.Add(RectBelow);
            if (RectBelow.Parent == null)
            {
                RectBelow.X = 75f;
            }
            else
            {
                RectBelow.RelativeX = 75f;
            }
            if (RectBelow.Parent == null)
            {
                RectBelow.Y = -140f;
            }
            else
            {
                RectBelow.RelativeY = -140f;
            }
            RectBelow.Width = 140f;
            RectBelow.Height = 140f;
            RectBelow.Visible = true;
            RectsList.Add(RectBelow2);
            if (RectBelow2.Parent == null)
            {
                RectBelow2.X = -75f;
            }
            else
            {
                RectBelow2.RelativeX = -75f;
            }
            if (RectBelow2.Parent == null)
            {
                RectBelow2.Y = -140f;
            }
            else
            {
                RectBelow2.RelativeY = -140f;
            }
            RectBelow2.Width = 140f;
            RectBelow2.Height = 140f;
            RectBelow2.Visible = true;
            RectsList.Add(RectTop);
            if (RectTop.Parent == null)
            {
                RectTop.X = -75f;
            }
            else
            {
                RectTop.RelativeX = -75f;
            }
            if (RectTop.Parent == null)
            {
                RectTop.Y = 140f;
            }
            else
            {
                RectTop.RelativeY = 140f;
            }
            RectTop.Width = 140f;
            RectTop.Height = 140f;
            RectTop.Visible = true;
            RectsList.Add(RectTop2);
            if (RectTop2.Parent == null)
            {
                RectTop2.X = 75f;
            }
            else
            {
                RectTop2.RelativeX = 75f;
            }
            if (RectTop2.Parent == null)
            {
                RectTop2.Y = 140f;
            }
            else
            {
                RectTop2.RelativeY = 140f;
            }
            RectTop2.Width = 140f;
            RectTop2.Height = 140f;
            RectTop2.Visible = true;
            RectsListUnused.Add(RectTopCenter);
            if (RectTopCenter.Parent == null)
            {
                RectTopCenter.X = 0f;
            }
            else
            {
                RectTopCenter.RelativeX = 0f;
            }
            if (RectTopCenter.Parent == null)
            {
                RectTopCenter.Y = 140f;
            }
            else
            {
                RectTopCenter.RelativeY = 140f;
            }
            RectTopCenter.Width = 140f;
            RectTopCenter.Height = 140f;
            RectTopCenter.Visible = false;
            RectsListUnused.Add(RectBelowCenter);
            if (RectBelowCenter.Parent == null)
            {
                RectBelowCenter.X = 0f;
            }
            else
            {
                RectBelowCenter.RelativeX = 0f;
            }
            if (RectBelowCenter.Parent == null)
            {
                RectBelowCenter.Y = -140f;
            }
            else
            {
                RectBelowCenter.RelativeY = -140f;
            }
            RectBelowCenter.Width = 140f;
            RectBelowCenter.Height = 140f;
            RectBelowCenter.Visible = false;
            if (RectLeftCenter.Parent == null)
            {
                RectLeftCenter.X = -140f;
            }
            else
            {
                RectLeftCenter.RelativeX = -140f;
            }
            if (RectLeftCenter.Parent == null)
            {
                RectLeftCenter.Y = 50f;
            }
            else
            {
                RectLeftCenter.RelativeY = 50f;
            }
            RectLeftCenter.Width = 140f;
            RectLeftCenter.Height = 140f;
            RectLeftCenter.Visible = false;
            if (RectRightCenter.Parent == null)
            {
                RectRightCenter.X = 140f;
            }
            else
            {
                RectRightCenter.RelativeX = 140f;
            }
            if (RectRightCenter.Parent == null)
            {
                RectRightCenter.Y = 50f;
            }
            else
            {
                RectRightCenter.RelativeY = 50f;
            }
            RectRightCenter.Width = 140f;
            RectRightCenter.Height = 140f;
            RectRightCenter.Visible = false;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = oldShapeManagerSuppressAdd;
        }
        public virtual void AddToManagersBottomUp () 
        {
            CameraSetup.ResetCamera(SpriteManager.Camera);
            AssignCustomVariables(false);
        }
        public virtual void RemoveFromManagers () 
        {
            for (int i = RectsList.Count - 1; i > -1; i--)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(RectsList[i]);
            }
            for (int i = RectsListUnused.Count - 1; i > -1; i--)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(RectsListUnused[i]);
            }
            ArrowsInstance.RemoveFromManagers();
            if (RectLeftCenter != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(RectLeftCenter);
            }
            if (RectRightCenter != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(RectRightCenter);
            }
        }
        public virtual void AssignCustomVariables (bool callOnContainedElements) 
        {
            if (callOnContainedElements)
            {
                ArrowsInstance.AssignCustomVariables(true);
            }
            if (RectMain.Parent == null)
            {
                RectMain.X = 0f;
            }
            else
            {
                RectMain.RelativeX = 0f;
            }
            if (RectMain.Parent == null)
            {
                RectMain.Y = 0f;
            }
            else
            {
                RectMain.RelativeY = 0f;
            }
            RectMain.Width = 140f;
            RectMain.Height = 140f;
            if (RectSide1.Parent == null)
            {
                RectSide1.X = -120f;
            }
            else
            {
                RectSide1.RelativeX = -120f;
            }
            if (RectSide1.Parent == null)
            {
                RectSide1.Y = 0f;
            }
            else
            {
                RectSide1.RelativeY = 0f;
            }
            RectSide1.Width = 100f;
            RectSide1.Height = 100f;
            if (RectSide2.Parent == null)
            {
                RectSide2.X = 120f;
            }
            else
            {
                RectSide2.RelativeX = 120f;
            }
            if (RectSide2.Parent == null)
            {
                RectSide2.Y = 0f;
            }
            else
            {
                RectSide2.RelativeY = 0f;
            }
            RectSide2.Width = 100f;
            RectSide2.Height = 100f;
            if (RectBelow.Parent == null)
            {
                RectBelow.X = 75f;
            }
            else
            {
                RectBelow.RelativeX = 75f;
            }
            if (RectBelow.Parent == null)
            {
                RectBelow.Y = -140f;
            }
            else
            {
                RectBelow.RelativeY = -140f;
            }
            RectBelow.Width = 140f;
            RectBelow.Height = 140f;
            RectBelow.Visible = true;
            if (RectBelow2.Parent == null)
            {
                RectBelow2.X = -75f;
            }
            else
            {
                RectBelow2.RelativeX = -75f;
            }
            if (RectBelow2.Parent == null)
            {
                RectBelow2.Y = -140f;
            }
            else
            {
                RectBelow2.RelativeY = -140f;
            }
            RectBelow2.Width = 140f;
            RectBelow2.Height = 140f;
            RectBelow2.Visible = true;
            if (RectTop.Parent == null)
            {
                RectTop.X = -75f;
            }
            else
            {
                RectTop.RelativeX = -75f;
            }
            if (RectTop.Parent == null)
            {
                RectTop.Y = 140f;
            }
            else
            {
                RectTop.RelativeY = 140f;
            }
            RectTop.Width = 140f;
            RectTop.Height = 140f;
            RectTop.Visible = true;
            if (RectTop2.Parent == null)
            {
                RectTop2.X = 75f;
            }
            else
            {
                RectTop2.RelativeX = 75f;
            }
            if (RectTop2.Parent == null)
            {
                RectTop2.Y = 140f;
            }
            else
            {
                RectTop2.RelativeY = 140f;
            }
            RectTop2.Width = 140f;
            RectTop2.Height = 140f;
            RectTop2.Visible = true;
            if (RectTopCenter.Parent == null)
            {
                RectTopCenter.X = 0f;
            }
            else
            {
                RectTopCenter.RelativeX = 0f;
            }
            if (RectTopCenter.Parent == null)
            {
                RectTopCenter.Y = 140f;
            }
            else
            {
                RectTopCenter.RelativeY = 140f;
            }
            RectTopCenter.Width = 140f;
            RectTopCenter.Height = 140f;
            RectTopCenter.Visible = false;
            if (RectBelowCenter.Parent == null)
            {
                RectBelowCenter.X = 0f;
            }
            else
            {
                RectBelowCenter.RelativeX = 0f;
            }
            if (RectBelowCenter.Parent == null)
            {
                RectBelowCenter.Y = -140f;
            }
            else
            {
                RectBelowCenter.RelativeY = -140f;
            }
            RectBelowCenter.Width = 140f;
            RectBelowCenter.Height = 140f;
            RectBelowCenter.Visible = false;
            if (RectLeftCenter.Parent == null)
            {
                RectLeftCenter.X = -140f;
            }
            else
            {
                RectLeftCenter.RelativeX = -140f;
            }
            if (RectLeftCenter.Parent == null)
            {
                RectLeftCenter.Y = 50f;
            }
            else
            {
                RectLeftCenter.RelativeY = 50f;
            }
            RectLeftCenter.Width = 140f;
            RectLeftCenter.Height = 140f;
            RectLeftCenter.Visible = false;
            if (RectRightCenter.Parent == null)
            {
                RectRightCenter.X = 140f;
            }
            else
            {
                RectRightCenter.RelativeX = 140f;
            }
            if (RectRightCenter.Parent == null)
            {
                RectRightCenter.Y = 50f;
            }
            else
            {
                RectRightCenter.RelativeY = 50f;
            }
            RectRightCenter.Width = 140f;
            RectRightCenter.Height = 140f;
            RectRightCenter.Visible = false;
        }
        public virtual void ConvertToManuallyUpdated () 
        {
            ArrowsInstance.ConvertToManuallyUpdated();
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
            TestBed.Entities.Arrows.LoadStaticContent(contentManagerName);
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
