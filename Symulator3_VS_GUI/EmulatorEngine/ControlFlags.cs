//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace EmulatorEngine
{
    public class CControlFlags
    {
        protected const int ONE_K_BYTES     = 1024;

        protected bool m_bExtendMnemonicBC  = false;
        protected bool m_bExtendMnemonicJC  = false;
        protected bool m_bExtendMnemonicMVX = false;
        protected bool m_bDasmCalledByEmu   = false;
        protected bool m_bInEmulator        = false;
        protected bool m_bInDynamicDASM     = false;
        protected volatile bool m_bStopRun  = false;

        protected string m_strTraceLogFilenamePrefix = @"D:\SoftwareDev\SacredCat\IBMSystem3\Data Files\Trace Logs\Trace";

        public void SetIsInEmulator (bool bInEmulator) { m_bInEmulator = bInEmulator; }
        public bool GetIsInEmulator () { return m_bInEmulator; }
        public void SetIsInDynamicDASM (bool bInDynamicDASM) { m_bInDynamicDASM = bInDynamicDASM; }

        private   bool m_bInTrace            = false;
        private   bool m_bTraceManuallyReset = false;
        public void SetTrace (bool bManuallyEnabled = false)
        {
            if (!m_bTraceManuallyReset ||
                bManuallyEnabled)
            {
                m_bInTrace            = true;
                m_bTraceManuallyReset = false;
            }
        }

        public void ResetTrace (bool bManuallyDisabled = false) { m_bInTrace = false; m_bTraceManuallyReset = bManuallyDisabled; }
        public bool IsInTrace ()         { return m_bInTrace; }
        public bool GetManuallyReset () { return m_bTraceManuallyReset; }

        private bool m_bStopOnHalt = false;
        public  void SetStopOnHalt ()   { m_bStopOnHalt = true; }
        public  void ResetStopOnHalt () { m_bStopOnHalt = false; }
        public  bool GetStopOnHalt ()   { return m_bStopOnHalt; }

        private bool m_bShow5475Halt = false;
        public  void SetShow5475Halt ()   { m_bShow5475Halt = true; }
        public  void ResetShow5475Halt () { m_bShow5475Halt = false; }
        public  bool GetShow5475Halt ()   { return m_bShow5475Halt; }

        private bool m_bShowCol61to64 = false;
        public  void SetShowCol61to64 ()   { m_bShowCol61to64 = true; }
        public  void ResetShowCol61to64 () { m_bShowCol61to64 = false; }
        public  bool GetShowCol61to64 ()   { return m_bShowCol61to64; }

        private bool m_bShowDisassembly = false;
        public  void SetShowDisassembly (bool bExtendedMnemonics = true)
        {
            m_bShowDisassembly = true;
            if (bExtendedMnemonics)
            {
                m_bExtendMnemonicBC  = true;
                m_bExtendMnemonicJC  = true;
                m_bExtendMnemonicMVX = true;
            }
        }
        public  void ResetShowDisassembly () { m_bShowDisassembly = false; }
        public  bool GetShowDisassembly ()   { return m_bShowDisassembly; }

        private bool m_bShowChangedValues = false;
        public  void SetShowChangedValues ()   { m_bShowChangedValues = true; }
        public  void ResetShowChangedValues () { m_bShowChangedValues = false; }
        public  bool GetShowChangedValues ()   { return m_bShowChangedValues; }

        private bool m_bShowMFCUBuffers = false;
        public  void SetShowMFCUBuffers ()   { m_bShowMFCUBuffers = true; }
        public  void ResetShowMFCUBuffers () { m_bShowMFCUBuffers = false; }
        public  bool GetShowMFCUBuffers ()   { return m_bShowMFCUBuffers; }

        private bool m_bShowPrinterBuffer = false;
        public  void SetShowPrinterBuffer ()   { m_bShowPrinterBuffer = true; }
        public  void ResetShowPrinterBuffer () { m_bShowPrinterBuffer = false; }
        public  bool GetShowPrinterBuffer ()   { return m_bShowPrinterBuffer; }

        private bool m_bShowDiskBuffers = false;
        public  void SetShowDiskBuffers ()   { m_bShowDiskBuffers = true; }
        public  void ResetShowDiskBuffers () { m_bShowDiskBuffers = false; }
        public  bool GetShowDiskBuffers ()   { return m_bShowDiskBuffers; }

        public void SetShowIOBuffers ()
        {
            SetShowMFCUBuffers ();
            SetShowPrinterBuffer ();
            SetShowDiskBuffers ();
        }

        public void ResetShowIOBuffers ()
        {
            ResetShowMFCUBuffers ();
            ResetShowPrinterBuffer ();
            ResetShowDiskBuffers ();
        }

        protected bool m_bEcho5471Keystrokes = false;
        public void SetEcho5471Keystrokes ()   { m_bEcho5471Keystrokes = true; }
        public void ResetEcho5471Keystrokes () { m_bEcho5471Keystrokes = false; }
        public bool GetEcho5471Keystrokes ()   { return m_bEcho5471Keystrokes; }

        protected bool m_bShowKeyboardIndicatorStatus = false;
        public void SetShowKeyboardIndicatorStatus ()   { m_bShowKeyboardIndicatorStatus = true; }
        public void ResetShowKeyboardIndicatorStatus () { m_bShowKeyboardIndicatorStatus = false; }
        public bool GetShowKeyboardIndicatorStatus ()   { return m_bShowKeyboardIndicatorStatus; }

        protected bool m_bLimitInstructionCount = false;
        public void SetLimitInstructionCount ()   { m_bLimitInstructionCount = true; }
        public void ResetLimitInstructionCount () { m_bLimitInstructionCount = false; }
        public bool GetLimitInstructionCount ()   { return m_bLimitInstructionCount; }

        protected bool m_bCpuMemTesting = false;
        public void SetCpuMemTesting ()   { m_bCpuMemTesting = true; }
        public void ResetCpuMemTesting () { m_bCpuMemTesting = false; }
        public bool GetCpuMemTesting ()   { return m_bCpuMemTesting; }

        protected bool m_bDelayInstructions = false;
        public void SetDelayInstructions ()   { m_bDelayInstructions = true; }
        public void ResetDelayInstructions () { m_bDelayInstructions = false; }
        public bool GetDelayInstructions ()   { return m_bDelayInstructions; }

        protected bool m_bSimulateSystem3CpuTiming = false;
        public void SetSimulateSystem3CpuTiming ()   { m_bSimulateSystem3CpuTiming = true;  m_bTestOneCardClockProgram = false; }
        public void ResetSimulateSystem3CpuTiming () { m_bSimulateSystem3CpuTiming = false; }
        public bool GetSimulateSystem3CpuTiming ()   { return m_bSimulateSystem3CpuTiming; }

        protected bool m_bShowRedFlag = false;
        public void SetShowRedFlag ()   { m_bShowRedFlag = true; }
        public void ResetShowRedFlag () { m_bShowRedFlag = false; }
        public bool GetShowRedFlag ()   { return m_bShowRedFlag; }

        protected bool m_bDebugDiskIPL = false;
        public void SetDebugDiskIPL ()   { m_bDebugDiskIPL = true; }
        public void ResetDebugDiskIPL () { m_bDebugDiskIPL = false; }
        public bool GetDebugDiskIPL ()   { return m_bDebugDiskIPL; }

        protected bool m_bTestOneCardClockProgram = false;
        public void SetTestOneCardClockProgram ()   { m_bTestOneCardClockProgram = true;  m_bSimulateSystem3CpuTiming = false;  }
        public void ResetTestOneCardClockProgram () { m_bTestOneCardClockProgram = false; }
        public bool GetTestOneCardClockProgram ()   { return m_bTestOneCardClockProgram; }

        protected bool m_bFix7SegmentDisplay = false;
        public void SetFix7SegmentDisplay ()   { m_bFix7SegmentDisplay = true; }
        public void ResetFix7SegmentDisplay () { m_bFix7SegmentDisplay = false; }
        public bool GetFix7SegmentDisplay ()   { return m_bFix7SegmentDisplay; }

        protected bool m_bEnableShow5475Status = false;
        public void EnableShow5475Status ()    { m_bEnableShow5475Status = true; }
        public void DisableShow5475Status ()   { m_bEnableShow5475Status = false; }
        public bool GetEnableShow5475Status () { return m_bEnableShow5475Status; }

        protected bool m_bAllowNonAsciiPrint = false;
        public void SetPrintNonAscii ()       { m_bAllowNonAsciiPrint = true; }
        public void ResetPrintNonAscii ()     { m_bAllowNonAsciiPrint = false; }
        public bool IsNowAsciiPrintAllowed () { return m_bAllowNonAsciiPrint;  }
    }
}
