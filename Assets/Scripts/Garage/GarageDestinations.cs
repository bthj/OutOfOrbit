using System;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("GarageDestination")]
public enum GarageDestinations {

	LOUNGE, CONTRACTS, PORTAL, SHOP, WINDOW
}