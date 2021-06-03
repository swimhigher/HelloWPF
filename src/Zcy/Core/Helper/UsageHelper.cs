using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Helper
{
    public static class UsageHelper
    {
        /// <summary>
        /// 检测间隔  
        /// </summary>
        private const int SampleRate = 1 * 1000; // 
        /// <summary>
        /// 数组最大值，最多存放多少个，为了计算快些 默认存放10分钟内的数据  也就是600的长度
        /// </summary>
        private const int CpuUsageListMaxCapacity = 10 * 60 * 1000 / SampleRate;

        private static long m_PhysicalMemory = 0;   //物理内存
        private static Timer _sampleTimer = null;
        private static readonly System.Diagnostics.PerformanceCounter _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        public static LinkedList<UseItemDto> _cpuUsageList = new LinkedList<UseItemDto>(); /*new LinkedList<float>();*/
        public static LinkedList<UseItemDto> _MemoryUsageList = new LinkedList<UseItemDto>();
        private static readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {
            //_cpuUsageList = new List<UseItemDto>();
            // _MemoryUsageList = new LinkedList<float>();
            if (_sampleTimer == null)
            {
                lock (_rwLock)
                {
                    if (_sampleTimer == null)
                    {
                        _cpuCounter.NextValue();
                        _sampleTimer = new Timer(ignored => sampleTask(), null, SampleRate, SampleRate);
                    }
                }

            }
            //获得物理内存
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["TotalPhysicalMemory"] != null)
                {
                    m_PhysicalMemory = long.Parse(mo["TotalPhysicalMemory"].ToString());
                }
            }
        }

        /// <summary>
        /// 获取最近多少秒内的平均值
        /// </summary>
        /// <param name="second"></param>
        /// <returns>[0.0, 100.0]</returns>
        public static float GetLastUsageAvg_Cpu(int second)
        {


            if (_sampleTimer == null)
            {
                throw new NullReferenceException("Please use Initialize() method on RTCA");
            }

            var capacity = second * 1000 / SampleRate;//有效数

            var result = -1f;

            try
            {
                _rwLock.EnterReadLock();

                if (_cpuUsageList.Count <= 0) return -1;

                var skip = Math.Max(_cpuUsageList.Count - capacity, 0);//有效数 的下标

                result = _cpuUsageList.Skip(skip).Select(p => p.value).Average();//有效数后面的list求平均
            }
            finally
            {
                _rwLock.ExitReadLock();
            }

            return result;
        }

        /// <summary>
        /// 获取最近多少秒内的平均值
        /// </summary>
        /// <param name="second"></param>
        /// <returns>[0.0, 100.0]</returns>
        public static float GetLastUsageAvg_Memory(int second)
        {


            var capacity = second * 1000 / SampleRate;//有效数

            var result = -1f;

            try
            {
                _rwLock.EnterReadLock();

                if (_MemoryUsageList.Count <= 0) return -1;

                var skip = Math.Max(_MemoryUsageList.Count - capacity, 0);//有效数 的下标

                result = _MemoryUsageList.Skip(skip).Select(p => p.value).Average();//有效数后面的list求平均
            }
            finally
            {
                _rwLock.ExitReadLock();
            }

            return result;
        }

        /// <summary>
        /// 获取最近n秒的最大值
        /// </summary>
        /// <param name="minutes">minutes must be in (0,60]</param>
        /// <returns>[0.0, 100.0]</returns>
        public static float GetLastUsageMax_Cpu(int seccond)
        {
            //if (seccond <= 0 || seccond > 60)
            //{
            //    throw new ArgumentOutOfRangeException("minutes");
            //}

            if (_sampleTimer == null)
            {
                throw new NullReferenceException("请调用 Initialize() method 初始化");
            }

            var capacity = seccond * 1000 / SampleRate;

            var result = -1f;
            try
            {
                _rwLock.EnterReadLock();

                if (_cpuUsageList.Count <= 0) return -1;

                var skip = Math.Max(_cpuUsageList.Count - capacity, 0);

                result = _cpuUsageList.Skip(skip).Select(p => p.value).Max();
            }
            finally
            {
                _rwLock.ExitReadLock();
            }

            return result;
        }

        /// <summary>
        /// 获取在过去{minutes}分钟内CPU使用率大于{threshold}%的比率
        /// </summary>
        /// <param name="minutes">minutes must be in (0,60]</param>
        /// <param name="threshold">threshold must be in [0.0,100.0]</param>
        /// <returns>[0.0, 1.0]</returns>
        public static float GetLastThresholdRatio_Cpu(int minutes, float threshold)
        {
            if (minutes <= 0 || minutes > 60)
            {
                throw new ArgumentOutOfRangeException("minutes");
            }

            if (threshold < 0f || threshold > 100f)
            {
                throw new ArgumentOutOfRangeException("threshold");
            }


            if (_sampleTimer == null)
            {
                throw new NullReferenceException("Please use Initialize() method on RTCA");
            }

            var capacity = minutes * 60 * 1000 / SampleRate;

            var result = -1f;
            try
            {
                _rwLock.EnterReadLock();

                var skip = Math.Max(_cpuUsageList.Count - capacity, 0);

                var count = _cpuUsageList.Skip(skip).Count(usage => usage.value >= threshold);

                result = count * 1f / (_cpuUsageList.Count - skip);
            }
            finally
            {
                _rwLock.ExitReadLock();
            }

            return result;
        }



        private static void sampleTask()
        {
            try
            {
                _rwLock.EnterWriteLock();

                if (_cpuUsageList.Count >= CpuUsageListMaxCapacity)
                {
                    _cpuUsageList.RemoveFirst();
                }
                _cpuUsageList.AddLast(new UseItemDto(DateTime.Now, _cpuCounter.NextValue()));

                if (_MemoryUsageList.Count >= CpuUsageListMaxCapacity)
                {
                    _MemoryUsageList.RemoveFirst();
                }
                _MemoryUsageList.AddLast(new UseItemDto(DateTime.Now, MemoryAvailabled));

            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }
        #region 可用内存
        /// <summary>
        /// 获取可用内存
        /// </summary>
        public static long MemoryAvailable
        {
            get
            {
                long availablebytes = 0;
                //ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_PerfRawData_PerfOS_Memory");
                //foreach (ManagementObject mo in mos.Get())
                //{
                //    availablebytes = long.Parse(mo["Availablebytes"].ToString());
                //}
                ManagementClass mos = new ManagementClass("Win32_OperatingSystem");
                foreach (ManagementObject mo in mos.GetInstances())
                {
                    if (mo["FreePhysicalMemory"] != null)
                    {
                        availablebytes = 1024 * long.Parse(mo["FreePhysicalMemory"].ToString());
                    }
                }
                return availablebytes;
            }
        }
        /// <summary>
        /// 获取已使用内存
        /// </summary>
        public static long MemoryAvailabled
        {
            get { return PhysicalMemory - MemoryAvailable; }
        }
        #endregion

        #region 物理内存
        /// <summary>
        /// 获取物理内存
        /// </summary>
        public static long PhysicalMemory
        {
            get
            {
                return m_PhysicalMemory;
            }
        }
        #endregion
        /// <summary>
        /// 检测网络延迟，-1 错误  0超时
        /// </summary>
        /// <param name="IpOrHost"></param>
        /// <returns>往返时间</returns>
        public static double PingTime(string IpOrHost, int timeout = 2000)
        {
            try
            {
                Ping pingSender = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true;
                string data = "0";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                PingReply reply = pingSender.Send(IpOrHost, timeout, buffer, options);
                return reply.Status == IPStatus.Success ? reply.RoundtripTime : 0;
            }
            catch (Exception)
            {

                return -1;
            }
        }


        ///  
        /// 获取磁盘使用情况  使用百分比
        ///  
        public static Dictionary<string, double> GetLogicalDrives()
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            try
            {
                var diskcon = Directory.GetLogicalDrives();
                if (diskcon != null && diskcon.Count() > 0)
                {
                    foreach (string diskname in diskcon)
                    {
                        DriveInfo di = new DriveInfo(diskname);
                        result.Add(di.Name?.Substring(0, 1), 1 - Math.Round((di.AvailableFreeSpace / (float)di.TotalSize), 3));
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return result;

        }
        ///  
        /// 获取磁盘使用情况  已用  未用
        ///  
        public static List<(string, float, float)> GetLogicalDrives_Use()
        {
            List<(string, float, float)> result = new List<(string, float, float)>();
            try
            {
                var diskcon = Directory.GetLogicalDrives();
                if (diskcon != null && diskcon.Count() > 0)
                {
                    foreach (string diskname in diskcon)
                    {
                        DriveInfo di = new DriveInfo(diskname);
                        result.Add((di.Name?.Substring(0, 1), (float)di.AvailableFreeSpace / 1073741824, (float)(di.TotalSize- di.AvailableFreeSpace) / 1073741824));
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return result;

        }

    }
    public class UseItemDto
    {
        public UseItemDto(DateTime datetime, float value)
        {
            this.datetime = datetime;
            this.value = value;
        }

        public DateTime datetime { get; set; }
        public float value { get; set; }
    }

}
