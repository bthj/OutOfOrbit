using System;

public enum Elements
{
    EARTH, FIRE, WATER, AIR
}

public static class ElementsMethods {

	public static Elements First(  ) {

		if( IsElementOrderDefined() ) {

			return GameStatus.instance.CurrentContract.Order[0];
		} else {

			return Elements.FIRE;
		}
	}

	public static Elements? Next( this Elements? type ) {

		Elements? nextElement = null;

		if( IsElementOrderDefined() ) {

			for( int i=0; i < GameStatus.instance.CurrentContract.Order.Count; i++ ) {
				
				if( type == GameStatus.instance.CurrentContract.Order[i] ) {
					if( i+1 < GameStatus.instance.CurrentContract.Order.Count ) {
						
						nextElement = GameStatus.instance.CurrentContract.Order[i+1];
					}
					break;
				}
			}
		} else {
		
			switch( type ) {
			case Elements.FIRE:
				nextElement = Elements.EARTH;
				break;
			case Elements.EARTH:
				nextElement = Elements.WATER;
				break;
			case Elements.WATER:
				nextElement = Elements.AIR;
				break;
			}
		}

		return nextElement;
	}

	private static bool IsElementOrderDefined() {

		return null != GameStatus.instance.CurrentContract.Order && 0 < GameStatus.instance.CurrentContract.Order.Count;
	}
}