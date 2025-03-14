using OpusSharp.Core.SafeHandlers;
using System;
using System.Text;

namespace OpusSharp.Core
{
    /// <summary>
    /// An opus decoder.
    /// </summary>
    public class OpusDecoder : IDisposable
    {
        /// <summary>
        /// Direct safe handle for the <see cref="OpusDecoder"/>. IT IS NOT RECOMMENDED TO CLOSE THE HANDLE DIRECTLY! Instead use <see cref="Dispose(bool)"/> to dispose the handle and object safely.
        /// </summary>
        protected OpusDecoderSafeHandle _handler;
        private bool _disposed;

        /// <summary>
        /// Gets the output sampling rate of the decoder.
        /// </summary>
        public int OutputSamplingRate { get; }

        /// <summary>
        /// Gets the number of channels of the decoder.
        /// </summary>
        public int OutputChannels { get; }

        /// <summary>
        /// Gets or sets the size of memory allocated for reading encoded data.
        /// 4000 is recommended.
        /// </summary>
        public int MaxDataBytes { get; set; }

        /// <summary>
        /// Creates a new opus decoder.
        /// </summary>
        /// <param name="sample_rate">The sample rate, this must be one of 8000, 12000, 16000, 24000, or 48000.</param>
        /// <param name="channels">Number of channels, this must be 1 or 2.</param>
        /// <exception cref="OpusException" />
        public unsafe OpusDecoder(int sample_rate, int channels)
        {
            int error = 0;
            _handler = NativeOpus.opus_decoder_create(sample_rate, channels, &error);
            CheckError(error);
            MaxDataBytes = 4000;
            OutputChannels = channels;
            OutputSamplingRate = sample_rate;
        }

        /// <summary>
        /// Opus decoder destructor.
        /// </summary>
        ~OpusDecoder()
        {
            Dispose(false);
        }

        /// <summary>
        /// Decodes an opus encoded frame.
        /// </summary>
        /// <param name="input">Input payload. Use null to indicate packet loss</param>
        /// <param name="length">Number of bytes in payload.</param>
        /// <param name="output">Output signal (interleaved if 2 channels). length is frame_size*channels*sizeof(short).</param>
        /// <param name="frame_size">Number of samples per channel of available space in pcm. If this is less than the maximum packet duration (120ms; 5760 for 48kHz), this function will not be capable of decoding some packets. In the case of PLC (data==NULL) or FEC (decode_fec=true), then frame_size needs to be exactly the duration of audio that is missing, otherwise the decoder will not be in the optimal state to decode the next incoming packet. For the PLC and FEC cases, frame_size must be a multiple of 2.5 ms.</param>
        /// <param name="decode_fec">Request that any in-band forward error correction data be decoded. If no such data is available, the frame is decoded as if it were lost.</param>
        /// <returns>Number of decoded samples or <see cref="OpusErrorCodes"/>.</returns>
#if NETSTANDARD2_0 || NET462_OR_GREATER
        public unsafe int Decode(byte[] input, int length, byte[] output, int frame_size, bool decode_fec)
#else
        public unsafe int Decode(Span<byte> input, int length, Span<byte> output, int frame_size, bool decode_fec)
#endif
        {
            ThrowIfDisposed();

            fixed (byte* inputPtr = input)
            fixed (byte* outputPtr = output)
            {
                var result = NativeOpus.opus_decode(_handler, inputPtr, length, (short*)outputPtr, frame_size, decode_fec ? 1 : 0);
                CheckError(result);
                return result;
            }
        }

        /// <summary>
        /// Decodes an opus encoded frame.
        /// </summary>
        /// <param name="input">Input payload. Use null to indicate packet loss</param>
        /// <param name="length">Number of bytes in payload.</param>
        /// <param name="output">Output signal (interleaved if 2 channels). length is frame_size*channels.</param>
        /// <param name="frame_size">Number of samples per channel of available space in pcm. If this is less than the maximum packet duration (120ms; 5760 for 48kHz), this function will not be capable of decoding some packets. In the case of PLC (data==NULL) or FEC (decode_fec=true), then frame_size needs to be exactly the duration of audio that is missing, otherwise the decoder will not be in the optimal state to decode the next incoming packet. For the PLC and FEC cases, frame_size must be a multiple of 2.5 ms.</param>
        /// <param name="decode_fec">Request that any in-band forward error correction data be decoded. If no such data is available, the frame is decoded as if it were lost.</param>
        /// <returns>Number of decoded samples or <see cref="OpusErrorCodes"/>.</returns>
#if NETSTANDARD2_0 || NET462_OR_GREATER
        public unsafe int Decode(byte[] input, int length, short[] output, int frame_size, bool decode_fec)
#else
        public unsafe int Decode(Span<byte> input, int length, Span<short> output, int frame_size, bool decode_fec)
#endif
        {
            ThrowIfDisposed();

            fixed (byte* inputPtr = input)
            fixed (short* outputPtr = output)
            {
                var result = NativeOpus.opus_decode(_handler, inputPtr, length, outputPtr, frame_size, decode_fec ? 1 : 0);
                CheckError(result);
                return result;
            }
        }

        /// <summary>
        /// Decodes an opus encoded frame.
        /// </summary>
        /// <param name="input">Input payload. Use null to indicate packet loss</param>
        /// <param name="length">Number of bytes in payload.</param>
        /// <param name="output">Output signal (interleaved if 2 channels). length is (frame_size*channels)/2. Note: I don't know if this is correct.</param>
        /// <param name="frame_size">Number of samples per channel of available space in pcm. If this is less than the maximum packet duration (120ms; 5760 for 48kHz), this function will not be capable of decoding some packets. In the case of PLC (data==NULL) or FEC (decode_fec=true), then frame_size needs to be exactly the duration of audio that is missing, otherwise the decoder will not be in the optimal state to decode the next incoming packet. For the PLC and FEC cases, frame_size must be a multiple of 2.5 ms.</param>
        /// <param name="decode_fec">Request that any in-band forward error correction data be decoded. If no such data is available, the frame is decoded as if it were lost.</param>
        /// <returns>Number of decoded samples or <see cref="OpusErrorCodes"/>.</returns>
#if NETSTANDARD2_0 || NET462_OR_GREATER
        public unsafe int Decode(byte[] input, int length, float[] output, int frame_size, bool decode_fec)
#else
        public unsafe int Decode(Span<byte> input, int length, Span<float> output, int frame_size, bool decode_fec)
#endif
        {
            ThrowIfDisposed();

            fixed (byte* inputPtr = input)
            fixed (float* outputPtr = output)
            {
                var result = NativeOpus.opus_decode_float(_handler, inputPtr, length, outputPtr, frame_size, decode_fec ? 1 : 0);
                CheckError(result);
                return result;
            }
        }

#if NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// Decodes an opus encoded frame.
        /// </summary>
        /// <param name="input">Input payload. Use null to indicate packet loss</param>
        /// <param name="length">Number of bytes in payload.</param>
        /// <param name="output">Output signal (interleaved if 2 channels). length is frame_size*channels</param>
        /// <param name="frame_size">Number of samples per channel of available space in pcm. If this is less than the maximum packet duration (120ms; 5760 for 48kHz), this function will not be capable of decoding some packets. In the case of PLC (data==NULL) or FEC (decode_fec=true), then frame_size needs to be exactly the duration of audio that is missing, otherwise the decoder will not be in the optimal state to decode the next incoming packet. For the PLC and FEC cases, frame_size must be a multiple of 2.5 ms.</param>
        /// <param name="decode_fec">Request that any in-band forward error correction data be decoded. If no such data is available, the frame is decoded as if it were lost.</param>
        /// <returns>Number of decoded samples or <see cref="OpusErrorCodes"/>.</returns>
        public unsafe int Decode(byte[]? input, int length, byte[] output, int frame_size, bool decode_fec) => Decode(input.AsSpan(), length, output.AsSpan(), frame_size, decode_fec);

        /// <summary>
        /// Decodes an opus encoded frame.
        /// </summary>
        /// <param name="input">Input payload. Use null to indicate packet loss</param>
        /// <param name="length">Number of bytes in payload.</param>
        /// <param name="output">Output signal (interleaved if 2 channels). length is frame_size*channels*sizeof(short)</param>
        /// <param name="frame_size">Number of samples per channel of available space in pcm. If this is less than the maximum packet duration (120ms; 5760 for 48kHz), this function will not be capable of decoding some packets. In the case of PLC (data==NULL) or FEC (decode_fec=true), then frame_size needs to be exactly the duration of audio that is missing, otherwise the decoder will not be in the optimal state to decode the next incoming packet. For the PLC and FEC cases, frame_size must be a multiple of 2.5 ms.</param>
        /// <param name="decode_fec">Request that any in-band forward error correction data be decoded. If no such data is available, the frame is decoded as if it were lost.</param>
        /// <returns>Number of decoded samples or <see cref="OpusErrorCodes"/>.</returns>
        public unsafe int Decode(byte[]? input, int length, short[] output, int frame_size, bool decode_fec) => Decode(input.AsSpan(), length, output.AsSpan(), frame_size, decode_fec);

        /// <summary>
        /// Decodes an opus encoded frame.
        /// </summary>
        /// <param name="input">Input payload. Use null to indicate packet loss</param>
        /// <param name="length">Number of bytes in payload.</param>
        /// <param name="output">Output signal (interleaved if 2 channels). length is frame_size*channels*sizeof(float)</param>
        /// <param name="frame_size">Number of samples per channel of available space in pcm. If this is less than the maximum packet duration (120ms; 5760 for 48kHz), this function will not be capable of decoding some packets. In the case of PLC (data==NULL) or FEC (decode_fec=true), then frame_size needs to be exactly the duration of audio that is missing, otherwise the decoder will not be in the optimal state to decode the next incoming packet. For the PLC and FEC cases, frame_size must be a multiple of 2.5 ms.</param>
        /// <param name="decode_fec">Request that any in-band forward error correction data be decoded. If no such data is available, the frame is decoded as if it were lost.</param>
        /// <returns>Number of decoded samples or <see cref="OpusErrorCodes"/>.</returns>
        public unsafe int Decode(byte[]? input, int length, float[] output, int frame_size, bool decode_fec) => Decode(input.AsSpan(), length, output.AsSpan(), frame_size, decode_fec);
#endif

        /// <summary>
        /// Performs a ctl request.
        /// </summary>
        /// <typeparam name="T">The type you want to input/output.</typeparam>
        /// <param name="request">The request you want to specify.</param>
        /// <param name="value">The input/output value.</param>
        /// <returns>The result code of the request. See <see cref="OpusErrorCodes"/>.</returns>
        /// <exception cref="OpusException" />
        /// <exception cref="ObjectDisposedException" />
        public unsafe int Ctl<T>(DecoderCTL request, ref T value) where T : unmanaged
        {
            ThrowIfDisposed();
            fixed (void* valuePtr = &value)
            {
                var result = NativeOpus.opus_decoder_ctl(_handler, (int)request, valuePtr);
                CheckError(result);
                return result;
            }
        }

        /// <summary>
        /// Performs a ctl request.
        /// </summary>
        /// <param name="request">The request you want to specify.</param>
        /// <returns>The result code of the request. See <see cref="OpusErrorCodes"/>.</returns>
        /// <exception cref="OpusException" />
        /// <exception cref="ObjectDisposedException" />
        public unsafe int Ctl(GenericCTL request)
        {
            ThrowIfDisposed();
            var result = NativeOpus.opus_decoder_ctl(_handler, (int)request);
            CheckError(result);
            return result;
        }

        /// <summary>
        /// Performs a ctl request.
        /// </summary>
        /// <typeparam name="T">The type you want to input/output.</typeparam>
        /// <param name="request">The request you want to specify.</param>
        /// <param name="value">The input/output value.</param>
        /// <returns>The result code of the request. See <see cref="OpusErrorCodes"/>.</returns>
        /// <exception cref="OpusException" />
        /// <exception cref="ObjectDisposedException" />
        public unsafe int Ctl<T>(GenericCTL request, ref T value) where T : unmanaged
        {
            ThrowIfDisposed();
            fixed (void* valuePtr = &value)
            {
                var result = NativeOpus.opus_decoder_ctl(_handler, (int)request, valuePtr);
                CheckError(result);
                return result;
            }
        }

        /// <summary>
        /// Produces PCM samples from Opus encoded data.
        /// </summary>
        /// <param name="inputOpusData">Opus encoded data to decode, null for dropped packet.</param>
        /// <param name="dataLength">Length of data to decode.</param>
        /// <param name="decodedLength">Set to the length of the decoded sample data.</param>
        /// <returns>PCM audio samples.</returns>
        public byte[] Decode(byte[] inputOpusData, int dataLength, out int decodedLength)
        {
            return Decode(inputOpusData, dataLength, out decodedLength, false);
        }

        /// <summary>
        /// Produces PCM samples from Opus encoded data.
        /// </summary>
        /// <param name="inputOpusData">Opus encoded data to decode, null for dropped packet.</param>
        /// <param name="dataLength">Length of data to decode.</param>
        /// <param name="decodedLength">Set to the length of the decoded sample data.</param>
        /// <param name="decode_fec">Request that any in-band forward error correction data be decoded. If no such data is available, the frame is decoded as if it were lost.</param>
        /// <returns>PCM audio samples.</returns>
        public unsafe byte[] Decode(byte[] inputOpusData, int dataLength, out int decodedLength, bool decode_fec)
        {
            ThrowIfDisposed();

            byte[] decoded = new byte[MaxDataBytes];
            int frameCount = FrameCount(MaxDataBytes);
            int length = Decode(inputOpusData, dataLength, decoded, frameCount, false);
            decodedLength = length * 2;
            CheckError(length);
            return decoded;
        }

        /// <summary>
        /// Determines the number of frames that can fit into a buffer of the given size.
        /// </summary>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public int FrameCount(int bufferSize)
        {
            //  seems like bitrate should be required
            int bitrate = 16;
            int bytesPerSample = (bitrate / 8) * OutputChannels;
            return bufferSize / bytesPerSample;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose logic.
        /// </summary>
        /// <param name="disposing">Set to true if fully disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (!_handler.IsClosed)
                    _handler.Close();
            }

            _disposed = true;
        }

        /// <summary>
        /// Throws an exception if this object is disposed or the handler is closed.
        /// </summary>
        /// <exception cref="ObjectDisposedException" />
        protected virtual void ThrowIfDisposed()
        {
            if (_disposed || _handler.IsClosed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        /// <summary>
        /// Checks if there is an opus error and throws if the error is a negative value.
        /// </summary>
        /// <param name="error">The error code to input.</param>
        /// <exception cref="OpusException"></exception>
        protected void CheckError(int error)
        {
            if (error < 0)
                throw new OpusException(((OpusErrorCodes)error).ToString());
        }
    }
}
