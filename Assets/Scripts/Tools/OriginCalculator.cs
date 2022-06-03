public class OriginCalculator
{
    public static MyObject.MyObject_LHW Calculate(MyObject.MyObject_LHW myObject_LHW, string type, string descriptor)
    {
        if (type == MyObject.OriginType.EDGE)
        { return EdgeOrigin(myObject_LHW, descriptor); }
        else if (type == MyObject.OriginType.PLANECENTER)
        { return PlaneCenterOrigin(myObject_LHW, descriptor); }
        else if (type == MyObject.OriginType.DIMENSIONCENTER)
        { return myObject_LHW.Zero(); }

        return myObject_LHW;
    }

    public static MyObject.MyObject_LHW Calculate(float l, float h, float w, string type, string descriptor)
    {
        MyObject.MyObject_LHW myObject_LHW = new(l, h, w);

        if (type == MyObject.OriginType.EDGE)
        { return EdgeOrigin(myObject_LHW, descriptor); }
        else if (type == MyObject.OriginType.PLANECENTER)
        { return PlaneCenterOrigin(myObject_LHW, descriptor); }
        else if (type == MyObject.OriginType.DIMENSIONCENTER)
        { return myObject_LHW.Zero(); }

        return myObject_LHW;
    }

    private static MyObject.MyObject_LHW EdgeOrigin(MyObject.MyObject_LHW myObject_LHW, string descriptor)
    {
        float l = myObject_LHW.L / 2;
        float h = myObject_LHW.H / 2;
        float w = myObject_LHW.W / 2;

        if (descriptor == MyObject.OriginDescriptor.LUF)
        { return new MyObject.MyObject_LHW(l, -h, -w); }

        if (descriptor == MyObject.OriginDescriptor.LUB)
        { return new MyObject.MyObject_LHW(l, -h, w); }

        if (descriptor == MyObject.OriginDescriptor.LDF)
        { return new MyObject.MyObject_LHW(l, h, -w); }

        if (descriptor == MyObject.OriginDescriptor.LDB)
        { return new MyObject.MyObject_LHW(l, h, w); }

        if (descriptor == MyObject.OriginDescriptor.RUF)
        { return new MyObject.MyObject_LHW(-l, -h, -w); }

        if (descriptor == MyObject.OriginDescriptor.RUB)
        { return new MyObject.MyObject_LHW(-l, -h, w); }

        if (descriptor == MyObject.OriginDescriptor.RDF)
        { return new MyObject.MyObject_LHW(-l, h, -w); }

        if (descriptor == MyObject.OriginDescriptor.RDB)
        { return new MyObject.MyObject_LHW(-l, h, w); }

        return new MyObject.MyObject_LHW(l, h, w);
    }

    private static MyObject.MyObject_LHW PlaneCenterOrigin(MyObject.MyObject_LHW myObject_LHW, string descriptor)
    {
        float l = myObject_LHW.L;
        float h = myObject_LHW.H;
        float w = myObject_LHW.W;

        if (descriptor == MyObject.OriginDescriptor.L)
        { return new MyObject.MyObject_LHW(l / 2, 0, 0); }

        if (descriptor == MyObject.OriginDescriptor.R)
        { return new MyObject.MyObject_LHW(-l / 2, 0, 0); }

        if (descriptor == MyObject.OriginDescriptor.U)
        { return new MyObject.MyObject_LHW(0, -h / 2, 0); }

        if (descriptor == MyObject.OriginDescriptor.D)
        { return new MyObject.MyObject_LHW(0, h / 2, 0); }

        if (descriptor == MyObject.OriginDescriptor.F)
        { return new MyObject.MyObject_LHW(0, 0, w / 2); }

        if (descriptor == MyObject.OriginDescriptor.B)
        { return new MyObject.MyObject_LHW(0, 0, -w / 2); }

        return new MyObject.MyObject_LHW(l, h, w);
    }
}
