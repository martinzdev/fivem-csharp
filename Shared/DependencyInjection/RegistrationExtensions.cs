namespace core.Shared.DependencyInjection
{
  public static class RegistrationExtensions
  {
    public static Registration SingleInstance(this Registration registration)
    {
      registration.SingleInstance = true;

      return registration;
    }

    public static Registration As<T>(this Registration registration)
    {
      var type = typeof(T);
      if (!registration.Types.Contains(type))
        registration.Types.Add(type);

      return registration;
    }
  }
}