using System;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("UPGRADE")]
public enum ShopItems {
	 /**
	  * TODO: not used yet, we might want to have one genereal thing
	  * to buy elements from the shop, whether they will be elements 
	  * or some other itmes, or we may just want to have separate implementations;
	  * for now the implementation, in Retail.cs, is just focused on element purchases...
	  */

	CHAIR,
	COOKIE_JAR, 
	LAMP, 
	POSTER, 
	PLANET_MACHINE,
	SPORTSCAR,
	TABLE_OBJECTS
}
