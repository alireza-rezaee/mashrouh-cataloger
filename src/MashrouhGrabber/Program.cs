using MashrouhGrabber;

var grabber = new Grabber();
await grabber.ReadCatalogue();
await grabber.Grab("./Mashrouh");