namespace GXPEngine.Core 
{	
	/// <summary>
	/// A class that contains info about collisions, such as returned by the MoveUntilCollision method.
	/// </summary>
	public class Collision 
	{
		public GameObject self,other;
		public Vec2 normal;
		public Vec2 point;
		public float timeOfImpact;
		public float penetrationDepth;

		public Collision(GameObject pSelf, GameObject pOther, Vec2 pNormal, Vec2 pPoint, float pTimeOfImpact, float pPenetrationDepth) 
		{
			self = pSelf;
			other = pOther;
			normal = pNormal;
			point = pPoint;
			timeOfImpact = pTimeOfImpact;
			penetrationDepth = pPenetrationDepth;
		}

		public Collision(GameObject pSelf, GameObject pOther, Vec2 pNormal, float pTimeOfImpact) :
			this (pSelf, pOther, pNormal, new Vec2 (0, 0), pTimeOfImpact, 0)
		{
		}

		public Collision(GameObject pSelf, GameObject pOther, Vec2 pNormal, Vec2 pPoint, float pPenetrationDepth) :
			this (pSelf, pOther, pNormal, pPoint, 0, pPenetrationDepth)
		{
		}			
	}
}
