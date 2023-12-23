using System;
using System.IO;

namespace shop
{
    public class EmptyResponse : FileNotFoundException
    {
        public override string Message
        {
            get { return "Have not found any cortages"; }
        }
    }

    public class NotEnoughExceptions : Exception
    {
        public override string Message
        {
            get { return "Not enough items to buying shipment"; }
        }
    }
}