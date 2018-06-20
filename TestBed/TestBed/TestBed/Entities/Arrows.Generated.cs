#if ANDROID || IOS || DESKTOP_GL
#define REQUIRES_PRIMARY_THREAD_LOADING
#endif
using Color = Microsoft.Xna.Framework.Color;
using TestBed.Screens;
using FlatRedBall.Graphics;
using FlatRedBall.Math;
using TestBed.Entities;
using FlatRedBall;
using FlatRedBall.Screens;
using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall.Math.Geometry;
namespace TestBed.Entities
{
    public partial class Arrows : FlatRedBall.PositionedObject, FlatRedBall.Graphics.IDestroyable
    {
        // This is made static so that static lazy-loaded content can access it.
        public static string ContentManagerName { get; set; }
        #if DEBUG
        static bool HasBeenLoadedWithGlobalContentManager = false;
        #endif
        static object mLockObject = new object();
        static System.Collections.Generic.List<string> mRegisteredUnloads = new System.Collections.Generic.List<string>();
        static System.Collections.Generic.List<string> LoadedContentManagers = new System.Collections.Generic.List<string>();
        
        private FlatRedBall.Math.Geometry.Polygon mArrowPoly1;
        public FlatRedBall.Math.Geometry.Polygon ArrowPoly1
        {
            get
            {
                return mArrowPoly1;
            }
            private set
            {
                mArrowPoly1 = value;
            }
        }
        private FlatRedBall.Math.Geometry.Polygon mArrowPoly4;
        public FlatRedBall.Math.Geometry.Polygon ArrowPoly4
        {
            get
            {
                return mArrowPoly4;
            }
            private set
            {
                mArrowPoly4 = value;
            }
        }
        private FlatRedBall.Math.Geometry.Polygon mArrowPoly2;
        public FlatRedBall.Math.Geometry.Polygon ArrowPoly2
        {
            get
            {
                return mArrowPoly2;
            }
            private set
            {
                mArrowPoly2 = value;
            }
        }
        private FlatRedBall.Math.Geometry.Polygon mArrowPoly3;
        public FlatRedBall.Math.Geometry.Polygon ArrowPoly3
        {
            get
            {
                return mArrowPoly3;
            }
            private set
            {
                mArrowPoly3 = value;
            }
        }
        protected FlatRedBall.Graphics.Layer LayerProvidedByContainer = null;
        public Arrows () 
        	: this(FlatRedBall.Screens.ScreenManager.CurrentScreen.ContentManagerName, true)
        {
        }
        public Arrows (string contentManagerName) 
        	: this(contentManagerName, true)
        {
        }
        public Arrows (string contentManagerName, bool addToManagers) 
        	: base()
        {
            ContentManagerName = contentManagerName;
            InitializeEntity(addToManagers);
        }
        protected virtual void InitializeEntity (bool addToManagers) 
        {
            LoadStaticContent(ContentManagerName);
            mArrowPoly1 = new FlatRedBall.Math.Geometry.Polygon();
            mArrowPoly1.Name = "mArrowPoly1";
            mArrowPoly4 = new FlatRedBall.Math.Geometry.Polygon();
            mArrowPoly4.Name = "mArrowPoly4";
            mArrowPoly2 = new FlatRedBall.Math.Geometry.Polygon();
            mArrowPoly2.Name = "mArrowPoly2";
            mArrowPoly3 = new FlatRedBall.Math.Geometry.Polygon();
            mArrowPoly3.Name = "mArrowPoly3";
            
            PostInitialize();
            if (addToManagers)
            {
                AddToManagers(null);
            }
        }
        public virtual void ReAddToManagers (FlatRedBall.Graphics.Layer layerToAddTo) 
        {
            LayerProvidedByContainer = layerToAddTo;
            FlatRedBall.SpriteManager.AddPositionedObject(this);
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(mArrowPoly1, LayerProvidedByContainer);
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(mArrowPoly4, LayerProvidedByContainer);
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(mArrowPoly2, LayerProvidedByContainer);
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(mArrowPoly3, LayerProvidedByContainer);
        }
        public virtual void AddToManagers (FlatRedBall.Graphics.Layer layerToAddTo) 
        {
            LayerProvidedByContainer = layerToAddTo;
            FlatRedBall.SpriteManager.AddPositionedObject(this);
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(mArrowPoly1, LayerProvidedByContainer);
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(mArrowPoly4, LayerProvidedByContainer);
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(mArrowPoly2, LayerProvidedByContainer);
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(mArrowPoly3, LayerProvidedByContainer);
            AddToManagersBottomUp(layerToAddTo);
            CustomInitialize();
        }
        public virtual void Activity () 
        {
            
            CustomActivity();
        }
        public virtual void Destroy () 
        {
            FlatRedBall.SpriteManager.RemovePositionedObject(this);
            
            if (ArrowPoly1 != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(ArrowPoly1);
            }
            if (ArrowPoly4 != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(ArrowPoly4);
            }
            if (ArrowPoly2 != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(ArrowPoly2);
            }
            if (ArrowPoly3 != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.Remove(ArrowPoly3);
            }
            CustomDestroy();
        }
        public virtual void PostInitialize () 
        {
            bool oldShapeManagerSuppressAdd = FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = true;
            if (mArrowPoly1.Parent == null)
            {
                mArrowPoly1.CopyAbsoluteToRelative();
                mArrowPoly1.AttachTo(this, false);
            }
            if (ArrowPoly1.Parent == null)
            {
                ArrowPoly1.X = 2f;
            }
            else
            {
                ArrowPoly1.RelativeX = 2f;
            }
            if (ArrowPoly1.Parent == null)
            {
                ArrowPoly1.Y = 58f;
            }
            else
            {
                ArrowPoly1.RelativeY = 58f;
            }
            if (ArrowPoly1.Parent == null)
            {
                ArrowPoly1.RotationZ = 1.57f;
            }
            else
            {
                ArrowPoly1.RelativeRotationZ = 1.57f;
            }
            FlatRedBall.Math.Geometry.Point[] ArrowPoly1Points = new FlatRedBall.Math.Geometry.Point[] {new FlatRedBall.Math.Geometry.Point(0, -50), new FlatRedBall.Math.Geometry.Point(0, 50), new FlatRedBall.Math.Geometry.Point(5, 35), new FlatRedBall.Math.Geometry.Point(-5, 35), new FlatRedBall.Math.Geometry.Point(0, 50) };
            ArrowPoly1.Points = ArrowPoly1Points;
            if (mArrowPoly4.Parent == null)
            {
                mArrowPoly4.CopyAbsoluteToRelative();
                mArrowPoly4.AttachTo(this, false);
            }
            if (ArrowPoly4.Parent == null)
            {
                ArrowPoly4.X = 2f;
            }
            else
            {
                ArrowPoly4.RelativeX = 2f;
            }
            if (ArrowPoly4.Parent == null)
            {
                ArrowPoly4.Y = -58f;
            }
            else
            {
                ArrowPoly4.RelativeY = -58f;
            }
            if (ArrowPoly4.Parent == null)
            {
                ArrowPoly4.RotationZ = -1.57f;
            }
            else
            {
                ArrowPoly4.RelativeRotationZ = -1.57f;
            }
            FlatRedBall.Math.Geometry.Point[] ArrowPoly4Points = new FlatRedBall.Math.Geometry.Point[] {new FlatRedBall.Math.Geometry.Point(0, -50), new FlatRedBall.Math.Geometry.Point(0, 50), new FlatRedBall.Math.Geometry.Point(5, 35), new FlatRedBall.Math.Geometry.Point(-5, 35), new FlatRedBall.Math.Geometry.Point(0, 50) };
            ArrowPoly4.Points = ArrowPoly4Points;
            if (mArrowPoly2.Parent == null)
            {
                mArrowPoly2.CopyAbsoluteToRelative();
                mArrowPoly2.AttachTo(this, false);
            }
            if (ArrowPoly2.Parent == null)
            {
                ArrowPoly2.X = 58f;
            }
            else
            {
                ArrowPoly2.RelativeX = 58f;
            }
            if (ArrowPoly2.Parent == null)
            {
                ArrowPoly2.Y = 1f;
            }
            else
            {
                ArrowPoly2.RelativeY = 1f;
            }
            FlatRedBall.Math.Geometry.Point[] ArrowPoly2Points = new FlatRedBall.Math.Geometry.Point[] {new FlatRedBall.Math.Geometry.Point(0, -50), new FlatRedBall.Math.Geometry.Point(0, 50), new FlatRedBall.Math.Geometry.Point(5, 35), new FlatRedBall.Math.Geometry.Point(-5, 35), new FlatRedBall.Math.Geometry.Point(0, 50) };
            ArrowPoly2.Points = ArrowPoly2Points;
            if (mArrowPoly3.Parent == null)
            {
                mArrowPoly3.CopyAbsoluteToRelative();
                mArrowPoly3.AttachTo(this, false);
            }
            if (ArrowPoly3.Parent == null)
            {
                ArrowPoly3.X = -58f;
            }
            else
            {
                ArrowPoly3.RelativeX = -58f;
            }
            if (ArrowPoly3.Parent == null)
            {
                ArrowPoly3.Y = 1f;
            }
            else
            {
                ArrowPoly3.RelativeY = 1f;
            }
            if (ArrowPoly3.Parent == null)
            {
                ArrowPoly3.RotationZ = 3.14f;
            }
            else
            {
                ArrowPoly3.RelativeRotationZ = 3.14f;
            }
            FlatRedBall.Math.Geometry.Point[] ArrowPoly3Points = new FlatRedBall.Math.Geometry.Point[] {new FlatRedBall.Math.Geometry.Point(0, -50), new FlatRedBall.Math.Geometry.Point(0, 50), new FlatRedBall.Math.Geometry.Point(5, 35), new FlatRedBall.Math.Geometry.Point(-5, 35), new FlatRedBall.Math.Geometry.Point(0, 50) };
            ArrowPoly3.Points = ArrowPoly3Points;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = oldShapeManagerSuppressAdd;
        }
        public virtual void AddToManagersBottomUp (FlatRedBall.Graphics.Layer layerToAddTo) 
        {
            AssignCustomVariables(false);
        }
        public virtual void RemoveFromManagers () 
        {
            FlatRedBall.SpriteManager.ConvertToManuallyUpdated(this);
            if (ArrowPoly1 != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(ArrowPoly1);
            }
            if (ArrowPoly4 != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(ArrowPoly4);
            }
            if (ArrowPoly2 != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(ArrowPoly2);
            }
            if (ArrowPoly3 != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(ArrowPoly3);
            }
        }
        public virtual void AssignCustomVariables (bool callOnContainedElements) 
        {
            if (callOnContainedElements)
            {
            }
            if (ArrowPoly1.Parent == null)
            {
                ArrowPoly1.X = 2f;
            }
            else
            {
                ArrowPoly1.RelativeX = 2f;
            }
            if (ArrowPoly1.Parent == null)
            {
                ArrowPoly1.Y = 58f;
            }
            else
            {
                ArrowPoly1.RelativeY = 58f;
            }
            if (ArrowPoly1.Parent == null)
            {
                ArrowPoly1.RotationZ = 1.57f;
            }
            else
            {
                ArrowPoly1.RelativeRotationZ = 1.57f;
            }
            if (ArrowPoly4.Parent == null)
            {
                ArrowPoly4.X = 2f;
            }
            else
            {
                ArrowPoly4.RelativeX = 2f;
            }
            if (ArrowPoly4.Parent == null)
            {
                ArrowPoly4.Y = -58f;
            }
            else
            {
                ArrowPoly4.RelativeY = -58f;
            }
            if (ArrowPoly4.Parent == null)
            {
                ArrowPoly4.RotationZ = -1.57f;
            }
            else
            {
                ArrowPoly4.RelativeRotationZ = -1.57f;
            }
            if (ArrowPoly2.Parent == null)
            {
                ArrowPoly2.X = 58f;
            }
            else
            {
                ArrowPoly2.RelativeX = 58f;
            }
            if (ArrowPoly2.Parent == null)
            {
                ArrowPoly2.Y = 1f;
            }
            else
            {
                ArrowPoly2.RelativeY = 1f;
            }
            if (ArrowPoly3.Parent == null)
            {
                ArrowPoly3.X = -58f;
            }
            else
            {
                ArrowPoly3.RelativeX = -58f;
            }
            if (ArrowPoly3.Parent == null)
            {
                ArrowPoly3.Y = 1f;
            }
            else
            {
                ArrowPoly3.RelativeY = 1f;
            }
            if (ArrowPoly3.Parent == null)
            {
                ArrowPoly3.RotationZ = 3.14f;
            }
            else
            {
                ArrowPoly3.RelativeRotationZ = 3.14f;
            }
        }
        public virtual void ConvertToManuallyUpdated () 
        {
            this.ForceUpdateDependenciesDeep();
            FlatRedBall.SpriteManager.ConvertToManuallyUpdated(this);
        }
        public static void LoadStaticContent (string contentManagerName) 
        {
            if (string.IsNullOrEmpty(contentManagerName))
            {
                throw new System.ArgumentException("contentManagerName cannot be empty or null");
            }
            ContentManagerName = contentManagerName;
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
            bool registerUnload = false;
            if (LoadedContentManagers.Contains(contentManagerName) == false)
            {
                LoadedContentManagers.Add(contentManagerName);
                lock (mLockObject)
                {
                    if (!mRegisteredUnloads.Contains(ContentManagerName) && ContentManagerName != FlatRedBall.FlatRedBallServices.GlobalContentManager)
                    {
                        FlatRedBall.FlatRedBallServices.GetContentManagerByName(ContentManagerName).AddUnloadMethod("ArrowsStaticUnload", UnloadStaticContent);
                        mRegisteredUnloads.Add(ContentManagerName);
                    }
                }
            }
            if (registerUnload && ContentManagerName != FlatRedBall.FlatRedBallServices.GlobalContentManager)
            {
                lock (mLockObject)
                {
                    if (!mRegisteredUnloads.Contains(ContentManagerName) && ContentManagerName != FlatRedBall.FlatRedBallServices.GlobalContentManager)
                    {
                        FlatRedBall.FlatRedBallServices.GetContentManagerByName(ContentManagerName).AddUnloadMethod("ArrowsStaticUnload", UnloadStaticContent);
                        mRegisteredUnloads.Add(ContentManagerName);
                    }
                }
            }
            CustomLoadStaticContent(contentManagerName);
        }
        public static void UnloadStaticContent () 
        {
            if (LoadedContentManagers.Count != 0)
            {
                LoadedContentManagers.RemoveAt(0);
                mRegisteredUnloads.RemoveAt(0);
            }
            if (LoadedContentManagers.Count == 0)
            {
            }
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
        protected bool mIsPaused;
        public override void Pause (FlatRedBall.Instructions.InstructionList instructions) 
        {
            base.Pause(instructions);
            mIsPaused = true;
        }
        public virtual void SetToIgnorePausing () 
        {
            FlatRedBall.Instructions.InstructionManager.IgnorePausingFor(this);
            FlatRedBall.Instructions.InstructionManager.IgnorePausingFor(ArrowPoly1);
            FlatRedBall.Instructions.InstructionManager.IgnorePausingFor(ArrowPoly4);
            FlatRedBall.Instructions.InstructionManager.IgnorePausingFor(ArrowPoly2);
            FlatRedBall.Instructions.InstructionManager.IgnorePausingFor(ArrowPoly3);
        }
        public virtual void MoveToLayer (FlatRedBall.Graphics.Layer layerToMoveTo) 
        {
            var layerToRemoveFrom = LayerProvidedByContainer;
            if (layerToRemoveFrom != null)
            {
                layerToRemoveFrom.Remove(ArrowPoly1);
            }
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(ArrowPoly1, layerToMoveTo);
            if (layerToRemoveFrom != null)
            {
                layerToRemoveFrom.Remove(ArrowPoly4);
            }
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(ArrowPoly4, layerToMoveTo);
            if (layerToRemoveFrom != null)
            {
                layerToRemoveFrom.Remove(ArrowPoly2);
            }
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(ArrowPoly2, layerToMoveTo);
            if (layerToRemoveFrom != null)
            {
                layerToRemoveFrom.Remove(ArrowPoly3);
            }
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(ArrowPoly3, layerToMoveTo);
            LayerProvidedByContainer = layerToMoveTo;
        }
    }
}
