using Eppy;
using RSG;
using UnityEngine;

public static partial class PromiseHelpers {
    public static IPromise<Tuple<T1, T2>> All<T1, T2>(IPromise<T1> p1, IPromise<T2> p2) {
        T1 val1 = default(T1);
        T2 val2 = default(T2);
        int numUnresolved = 2;
        var promise = new Promise<Tuple<T1, T2>>();

        p1
            .Catch((e) => {
                promise.Reject(e);
            })
            .Done((val) => {
                val1 = val;
                numUnresolved--;
                if (numUnresolved <= 0) promise.Resolve(new Tuple<T1, T2>(val1, val2));
            });

        p2
            .Catch((e) => {
                promise.Reject(e);
            })
            .Done((val) => {
                val2 = val;
                numUnresolved--;
                if (numUnresolved <= 0) promise.Resolve(new Tuple<T1, T2>(val1, val2));
            });

        return promise;
    }

    public static IPromise<Tuple<T1, T2, T3>> All<T1, T2, T3>(IPromise<T1> p1, IPromise<T2> p2, IPromise<T3> p3) {
        return All(All(p1, p2), p3)
            .Then((vals) => {
                return new Tuple<T1, T2, T3>(vals.Item1.Item1, vals.Item1.Item2, vals.Item2);
            });
    }

    public static IPromise<Tuple<T1, T2, T3, T4>> All<T1, T2, T3, T4>(IPromise<T1> p1, IPromise<T2> p2, IPromise<T3> p3, IPromise<T4> p4) {
        return All(All(p1, p2), All(p3, p4))
            .Then((vals) => {
                return new Tuple<T1, T2, T3, T4>(vals.Item1.Item1, vals.Item1.Item2, vals.Item2.Item1, vals.Item2.Item2);
            });
    }
}
