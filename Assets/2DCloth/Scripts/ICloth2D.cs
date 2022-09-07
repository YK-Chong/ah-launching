namespace Cloth2D
{
    public interface ICloth2D
    {
        Cloth2DJoint[] GetAllJoints();
        Cloth2DJoint[] GetTopJoints();
        Cloth2DJoint[] GetBottomJoints();
        Cloth2DJoint[] GetLeftJoints();
        Cloth2DJoint[] GetRightJoints();
        
        Cloth2DJoint[] GenerateJoints();
        void DestroyJoints();
    }
}