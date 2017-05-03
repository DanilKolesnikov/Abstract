namespace ConsolCourse
{
    static class Hash
    {
        private const int b = 37;
        private const int m = 1009;

        public static int hashFunc(string s)
        {
            int cb = 1;
            int ans = 0;
            for (int i = 0; i < s.Length; ++i)
            {
                ans = (ans + (int)(s[i] - 'a') * cb % m) % m;
                cb = cb * b % m;
            }
            return ans;
        }
    }
}
