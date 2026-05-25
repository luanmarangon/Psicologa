import React from "react";

export default function ApresentarImagem({
  src,
  width = "100%",
  height = "200px",
  alt = "imagem",
  style = {},
}) {
  const getImageSrc = () => {
    if (!src) return null;

    // base64 completo
    if (src.startsWith("data:image")) return src;

    // base64 cru
    if (/^[A-Za-z0-9+/=]+$/.test(src)) {
      return `data:image/png;base64,${src}`;
    }

    // URL normal
    return src;
  };

  const finalSrc = getImageSrc();

  // 👉 Se não tiver imagem → mostra placeholder
  if (!finalSrc) {
    return (
      <div
        style={{
          width,
          height,
          backgroundColor: "#f0f0f0",
          color: "#999",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          fontSize: "14px",
          borderRadius: "8px",
          ...style,
        }}
      >
        Sem Imagem
      </div>
    );
  }

  return (
    <img
      src={finalSrc}
      alt={alt}
      style={{
        width,
        height,
        objectFit: "cover",
        display: "block",
        borderRadius: "8px",
        ...style,
      }}
      onError={(e) => {
        e.target.style.display = "none";

        const fallback = document.createElement("div");
        fallback.innerText = "Sem Imagem";
        fallback.style.width = width;
        fallback.style.height = height;
        fallback.style.background = "#f0f0f0";
        fallback.style.color = "#999";
        fallback.style.display = "flex";
        fallback.style.alignItems = "center";
        fallback.style.justifyContent = "center";
        fallback.style.borderRadius = "8px";

        e.target.parentNode.appendChild(fallback);
      }}
    />
  );
}