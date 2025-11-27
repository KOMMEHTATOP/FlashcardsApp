export default function SkeletonGroupDetail() {
  return (
    <div className="min-h-screen animate-pulse">
      <div className="relative bg-base-200 px-4 sm:px-6 lg:px-8 py-12 overflow-hidden rounded-2xl shadow-xl">
        <div className="absolute inset-0 bg-base-300/20" />
        <div className="max-w-7xl mx-auto relative z-10">
          <div className="mb-6 flex items-center w-fit">
            <div className="w-5 h-5 bg-base-300 rounded mr-2 skeleton"></div>
            <div className="bg-base-300 rounded h-4 w-32 skeleton"></div>
          </div>

          <div className="flex flex-col sm:flex-row items-start sm:items-center gap-6 mb-8">
            <div className="bg-base-300 p-6 rounded-3xl w-20 h-20 skeleton"></div>

            <div className="flex-1">
              <div className="flex items-center gap-3 mb-2 ">
                <div className="bg-base-300 rounded h-10 w-48 skeleton"></div>
                <div className="bg-base-300 px-3 py-2 rounded-full w-32 h-8 skeleton"></div>
              </div>

              <div className="bg-base-300 rounded h-6 w-64 mb-4 skeleton"></div>

              <div className="space-y-2">
                <div className="flex justify-between">
                  <div className="bg-base-300 rounded h-4 w-24 skeleton"></div>
                  <div className="bg-base-300 rounded h-4 w-12 skeleton"></div>
                </div>
                <div className="w-full h-2 bg-base-300 rounded-full skeleton"></div>
              </div>
            </div>
          </div>

          <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
            {[...Array(4)].map((_, index) => (
              <div
                key={index}
                className="bg-base-300 rounded-2xl p-4 text-center skeleton"
              >
                <div className="bg-base-200 rounded h-8 w-12 mx-auto mb-1"></div>
                <div className="bg-base-200 rounded h-4 w-16 mx-auto"></div>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4">
          <div className="bg-base-200 rounded h-8 w-32 skeleton"></div>

          <div className="grid grid-cols-1 sm:grid-cols-3 md:flex md:flex-row md:items-center gap-3 md:gap-4 w-full md:w-auto">
            <div className="flex justify-center md:justify-start">
              <div className="bg-base-200 rounded-xl w-32 h-10 skeleton"></div>
            </div>

            <div className="flex items-center justify-center md:justify-start gap-2 ">
              <div className="w-5 h-5 bg-base-200 rounded"></div>
              <div className="bg-base-200 rounded h-4 w-28 skeleton"></div>
            </div>
          </div>
        </div>

        <div className="space-y-4">
          {[...Array(3)].map((_, index) => (
            <div
              key={index}
              className="bg-base-200 rounded-2xl p-6 space-y-4 skeleton"
            >
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="bg-base-300 rounded-full w-12 h-12"></div>
                  <div className="space-y-2">
                    <div className="bg-base-300 rounded h-4 w-32"></div>
                    <div className="bg-base-300 rounded h-3 w-24"></div>
                  </div>
                </div>
                <div className="flex gap-2">
                  <div className="bg-base-300 rounded w-20 h-10"></div>
                </div>
              </div>
            </div>
          ))}
        </div>

        <div className="mt-8 bg-base-200 rounded-2xl p-12 items-center flex flex-col skeleton">
          <div className="flex items-center gap-3 mb-3 ">
            <div className="space-y-2 flex-1 flex flex-col items-center">
              <div className="bg-base-300 rounded h-5 w-100"></div>
              <div className="bg-base-300 rounded h-4 w-150"></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
