
namespace IAG.Infrastructure.Test.DI.TestData;
/* BaseContextImpl
 *   ^
 *   |-- BaseContext
 *   |-- SpecificContextOneImpl
 *   |     ^
 *   |     |- SpecificContextOne
 *   |-- SpecificContextTwoImpl
 *         ^
 *         |- SpecificContextTwo<A>
 *         |-- SpecificContextThreeImpl
 *              ^
 *              |-- SpecificContextThree<B>
 */

public class A { }
public class B : A { }

public class BaseContext : BaseContextImpl { }

public class BaseContextImpl { }

public class SpecificContextOne : SpecificContextOneImpl { }

public abstract class SpecificContextOneImpl : BaseContextImpl { }

public class SpecificContextTwo : SpecificContextTwoImpl<A> { }

// ReSharper disable once UnusedTypeParameter
public abstract class SpecificContextTwoImpl<T> : BaseContextImpl { }

public class SpecificContextThree : SpecificContextThreeImpl<B> { }

public abstract class SpecificContextThreeImpl<T> : SpecificContextTwoImpl<T> { }